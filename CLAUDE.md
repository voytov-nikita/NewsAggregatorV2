# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

News aggregator with a microservices backend (.NET 9 / C#) and an Angular 21 frontend. Four backend services; three of them communicate via RabbitMQ message queues:

- **NewsService** — Main API for aggregated news and comments. PostgreSQL via EF Core. Consumes `news-queue` (from CrawlerService), produces to `webhooks-queue` (for NotificationService).
- **CrawlerService** — Scheduled RSS crawler. MongoDB. Uses Hangfire for job scheduling. Produces to `news-queue`.
- **NotificationService** — Webhook/notification delivery. MongoDB. Consumes `webhooks-queue`.
- **AuthService** — Authentication and permission-based authorization. PostgreSQL via EF Core + ASP.NET Core Identity. Issues RS256 JWTs; no message queue.
- **NewsAggregatorClient** — Angular 21 SPA frontend.

## Repository Layout

```
src/server/common/          Shared libraries (Common, Common.API, Common.Auth, Logger, API.Logger, MessageQueue)
src/server/newsService/     NewsService.API / .BLL / .DAL.PostgreSql / .Models
src/server/crawlerService/  CrawlerService.API / .BLL / .DAL / .Hangfire / .Models
src/server/notificationService/  NotificationService.API / .BLL / .DAL / .Models
src/server/authService/     AuthService.API / .BLL / .DAL.PostgreSql / .Models
src/client/NewsAggregatorClient/ Angular frontend
tests/newsService/          NewsService.BLL.Tests
tests/crawlerService/       API.Tests, BLL.Tests, DAL.IntegrationTests
tests/notificationService/  NotificationService.BLL.Tests
tests/authService/          AuthService.BLL.Tests
tests/common/               Common.Auth.Tests
docs/Solution/              Architecture docs and diagrams
```

Each backend service has its own solution file at the repo root: `NewsService.sln`, `CrawlerService.sln`, `NotificationService.sln`, `AuthService.sln`.

HTTPS ports: NewsService 7300, CrawlerService 7310, NotificationService 7320, AuthService 7330. All launch profiles set `ASPNETCORE_ENVIRONMENT=Local`, which is **not** `Development` — code that should run locally must check `IsEnvironment("Local")` too (Swagger, user secrets).

## Infrastructure (Docker)

Services depend on: PostgreSQL (port 5432), MongoDB (port 27017), LavinMQ/RabbitMQ (ports 5672/15672), Seq logging (port 5341). See `readme.md` for docker run commands.

## Architecture Patterns

**Layered architecture** per service: API → BLL (Business Logic) → DAL (Data Access) with abstractions interfaces separating each layer.

**Dependency injection** via `ServiceCollectionExtensions` in each layer, composed in `Program.cs`:
```csharp
builder.Services.AddBusinessLayer();
builder.Services.AddDataAccessLayer(connectionString);
builder.Services.AddNewsServiceConsumers(queueSettings);
```

**Message queue abstraction**: `IMessageProducer` / `IMessageConsumer` in `Common/MessageQueue/` with Polly resilience policies. Queue names: `news-queue`, `webhooks-queue`, `webhook-triggered`.

**Webhook dispatch flow**: `IWebhookDispatcher.Dispatch<T>(eventType, data)` → `webhooks-queue` → `WebhooksProcessor` (subscription lookup) → `webhook-triggered` queue → `WebhookTriggeredProcessor` (HTTP delivery via `IWebhookDeliveryService`).

**Authorization**: AuthService (schema `authService` in the shared `local-news` database) owns users, roles, permissions and refresh tokens via ASP.NET Core Identity with `Guid` keys. Permissions are real tables (`Permissions`, `RolePermissions`) rather than `AspNetRoleClaims`, seeded on startup from `Permissions.All` by ordered `IDataSeeder`s run inside `MigrationsFilter`; the admin user comes from `Seed:AdminEmail`/`Seed:AdminPassword` in user secrets and is skipped when unset. Refresh tokens are stored as SHA-256 hashes with a `FamilyId` rotation chain for reuse detection.

**Token issuing**: RS256. `SigningKeyProvider` (singleton, BLL) loads `Jwt:KeysPath/signing-key.pem` or generates and persists one on first run — the key must survive restarts or every outstanding token and cached JWKS breaks. `kid` is derived from the public key (SHA-256 of `SubjectPublicKeyInfo`), so it is stable across restarts; a random per-startup `kid` would silently invalidate tokens. Rotation: move the active key to `keys/retired/<kid>.pem` and restart — retired keys stay published in JWKS while the active one signs. `keys/` and `*.pem` are gitignored. `TokenService` (`JsonWebTokenHandler`) issues 15-minute access tokens with `sub`/`email`/`name`/`jti` + one `role` and one `permission` claim each, and 32-byte refresh tokens hashed with plain SHA-256. Discovery lives at `/.well-known/openid-configuration` and `/.well-known/jwks.json`; JWKS is built from public-only RSA parameters rather than `JsonWebKeyConverter`, which would include private components. `ISigningKeyProvider` stays inside the BLL so IdentityModel types do not leak into the abstractions — the API consumes `IJwksService`, which returns plain models.

**Auth API** (`api/v1/auth`): `register` / `login` / `refresh` / `logout` are anonymous, `me` requires `[Authorize]`. The refresh token is **never** in a response body — it only travels in the `na_rt` httpOnly + Secure + `SameSite=None` cookie scoped to `/api/v1/auth`, so a XSS cannot read it. Login failures are uniform (`401 Invalid email or password`) whether the email is unknown or the password wrong, to avoid user enumeration; lockout is `403`. Expected failures come back as `AuthErrorCode` values on `AuthResultModel` and are mapped to status codes in the controller — the repo's usual "validator throws `ArgumentException`" style would turn a wrong password into a 500 via `UseExceptionHandler`; `IAuthValidator` still throws, but only for shape problems (empty/malformed email, empty password). Password length and complexity stay in Identity's `PasswordOptions` so the rules live in one place. Refresh rotates on every use and marks the old row `Rotated` with `ReplacedByTokenId`; presenting an already-revoked token means it leaked, so the whole `FamilyId` chain is revoked as `ReuseDetected` and logged as a warning. Logout revokes the refresh token (`?allDevices=true` revokes all) but the access token stays valid until it expires — inherent to stateless JWT, mitigated by the 15-minute lifetime. AuthService is the only service whose CORS policy enumerates origins (`Cors:AllowedOrigins`) and sets `AllowCredentials`, because `AllowAnyOrigin()` is rejected by browsers alongside credentials; the others keep `AllowAll`. It also calls `AddJwtAuthentication` against itself so `[Authorize]` works on `me`, and adds a bearer scheme to the OpenAPI document so Swagger's Authorize button appears.

`common/Common.Auth` — `AddJwtAuthentication(AuthSettings)` wires `AddJwtBearer` against AuthService's JWKS (`Authority` + `Audience`, `MapInboundClaims = false`, 30s clock skew). Permission-based, not role-based: roles map to permissions, permissions travel as `permission` claims, and endpoints opt in with `[HasPermission(Permissions.CommentWrite)]`. `PermissionPolicyProvider` materializes `perm:<name>` policies on demand, so no policy is registered per permission. `ICurrentUser` (scoped) exposes the caller's id/email/roles/permissions to BLL without touching `HttpContext`. Permission and role catalogues are constants in `Common.Auth/Constants/`. Reads are anonymous by design — there is no global authorize filter.

All four services call `AddJwtAuthentication(globalSettings.Auth)` and put `.UseAuthentication().UseAuthorization()` between `UseRouting()` and `UseResponseCompression()` — `UseCors` stays ahead of both so a 401 still carries CORS headers, and `UseRouting` ahead of authorization so `[HasPermission]` metadata is resolved when it runs. The `Auth` section (`Authority` `https://localhost:7330`, `Audience` `news-aggregator`, `RequireHttpsMetadata`) lives in each service's `appsettings.json` and binds through its own `GlobalSettings`; `Authority` must match AuthService's `Jwt:Issuer` byte for byte or validation fails with `IDX10205`. Consumers fetch JWKS over HTTPS, so `dotnet dev-certs https --trust` is required locally. `Common.API.AddCustomControllers` deliberately registers no global `AuthorizeFilter` — read APIs are public, and a service that must be closed end to end sets its own `FallbackPolicy` in its `Program.cs`.

**NewsService writes**: comments carry `AuthorId` (Guid) plus `AuthorName`, a denormalized snapshot of the `name` claim taken at write time — NewsService never calls AuthService synchronously to render a list, and a foreign key cannot cross a service boundary; renaming a user therefore does not rename their old comments. The author is always filled from `ICurrentUser` in the controller, never from the request body. `POST comments` needs `comment.write`; `PUT`/`DELETE`/like/dislike need `[Authorize]`, and `CommentsService` (BLL) then allows the author or anyone holding `comment.moderate`. Reads stay anonymous. Votes live in `Votes` (`NewsId`, `UserId`, `Value` +1/-1) with `UNIQUE (NewsId, UserId)`, which is what makes `POST /api/v1/news/{id}/vote` (`news.vote`, body `{ value: 1 | -1 | 0 }`, 0 retracts) idempotent; `News.Likes`/`Dislikes` stay denormalized and are recounted from `Votes` in the same transaction, wrapped in `CreateExecutionStrategy()` because `EnableRetryOnFailure` rejects user-initiated transactions otherwise. `DomainExceptionHandler` (`IExceptionHandler`) maps `UnauthorizedAccessException` → 403, `KeyNotFoundException` → 404, `ArgumentException` → 400; without it `UseExceptionHandler` would return 500 for all three.

**Testing stack**: xUnit + Moq + AutoFixture + FluentAssertions. Angular uses Vitest + Jsdom.

**Logging**: Serilog in `common/Logger` (configured via `serilogsettings.{env}.json`), API-specific wiring in `common/API.Logger`. All services ship to Seq (`http://localhost:5341`) and Console. `ConfigureCommonApiSettings()` calls `builder.UseLogger()`. Runtime log level is controlled by `ILogLevelService` (bound to `$controlSwitch`).

## Angular Client

Angular 21 SPA, NgModule-based architecture. Auth is a signal-based `AuthService` with a single-flight refresh interceptor, `authGuard` / `permissionGuard`, and the access token kept in memory only. Details in [`src/client/NewsAggregatorClient/CLAUDE.md`](src/client/NewsAggregatorClient/CLAUDE.md).

## Available Skills

Only `create-project` uses the auto-discovered layout (`.claude/skills/<name>/SKILL.md`); the others are flat
`.md` files that are not registered as invocable skills — reference them by path.

- **create-project**: `.claude/skills/create-project/SKILL.md` (+ `templates/`)
  Scaffold a new three-layer service (API / BLL / BLL.Abstractions / DAL / DAL.Abstractions / Models), its `.sln` and BLL test project. Accepts service name and optional `db` (`postgres` default, or `mongo`). Picks a free 73x0 port automatically.
- **create-migration**: `.claude/skills/create-migration.md`
  Use when in need to create EF Core migrations for any service with PostgreSQL DAL. Accepts migration name and optional service name.
- **create-integration-test**: `.claude/skills/create-integration-test.md`
  Placeholder for integration test creation (DAL stores, API controllers). Not yet implemented — skips and logs.
- **create-unit-test**: `.claude/skills/create-unit-test.md`
  Create or update a unit test file for a source file. Accepts file path and optional method name.
- **up-to-date-tests**: `.claude/skills/up-to-date-tests.md`
  Verify test coverage is up-to-date for a service. Checks BLL services, validators, extensions, mappers, DAL stores, and API controllers against existing tests. Accepts service name. 


# Important — MUST follow without being asked

ALWAYS update this CLAUDE.md immediately after any change that affects architecture, models, commands, environments, or deployment — without waiting for the user to ask. Keep additions concise and to the point: one fact per line, no redundant context.

If asked to review CLAUDE.md for accuracy, read the current state of the project and update any outdated information.
