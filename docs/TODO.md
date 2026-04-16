# TODO

## NewsService

### News categories

Add category system for aggregated news — taxonomy (sport, politics, tech, etc.) and
per-article categorization to allow filtering/grouping on the frontend.

**Scope:**
- `Category` table and `NewsCategories` join table (many-to-many) in NewsService
- EF Core migration
- Extend `news-queue` message to carry categories from CrawlerService (mapping per source)
- `GET /api/v1/news` — filter by `categoryId`
- Seed a default category list or derive from source tags

### News rating

Implement a rating system for news items (likes/dislikes, or 1-5 stars).

**Considerations:**
- Anonymous vs. authenticated voting — tied to the upcoming Authorization Service
- Counter storage on `News` or separate `NewsRating` table
- Anti-spam: one vote per user/IP per article

### News validation

Server-side validation rules for incoming news from `news-queue` (title/description length,
required fields, URL format, publish date sanity). Currently we trust whatever CrawlerService
sends.

### Normalize publisher storage

Currently the `News` table duplicates publisher info (`PublisherName`, `PublisherLink`, `PublisherGuid`)
on every row. With multiple sources coming from CrawlerService, this duplication grows linearly with
the number of articles.

**Idea:** Extract a separate `Publishers` (or `Sources`) table in NewsService and reference it by FK
from `News`. Each article would store only a `SourceId` / `PublisherId`, and publisher metadata lives
in one row per source.

**Considerations:**
- NewsService currently has no concept of a source — publisher arrives as denormalized fields in
  the `news-queue` message. Either propagate `SourceId` end-to-end (CrawlerService → queue → NewsService),
  or have NewsService upsert a `Publisher` row on first sight of a new `PublisherName`.
- Requires an EF Core migration (`dotnet ef migrations add NormalizePublisher`) and a data backfill
  for existing rows.
- Frontend DTOs may need to join publisher info — keep denormalized at the read model if performance matters.

## CrawlerService

### Cross-source deduplication

Currently, deduplication is based on `GlobalUniqueId` (publisher name + guid from RSS feed).
This only catches exact duplicates from the same source.

When multiple sources are added, the same news story may appear from different publishers
(e.g., Pravda and BBC both report the same event). These are currently treated as separate news items.

**Idea:** Implement cross-source deduplication by comparing:
- News titles (fuzzy matching / similarity score)
- Common tags or categories
- Combination of both

This would allow grouping related articles from different sources
and avoiding duplicate stories in the aggregated feed.

**Considerations:**
- Fan-in pattern may be needed if deduplication requires data from all sources at once
- Alternatively, compare against already-saved news in the database (no fan-in needed)
- Fuzzy matching adds complexity — consider a similarity threshold

### Source management UI (step 2 of source refactor)

After step 1 (universal parser with mapping, sources stored in Mongo) is complete,
add a frontend for managing sources without touching code or the database directly.

**Scope:**
- CRUD API in `CrawlerService.API` for `NewsSource` (list, create, update, delete, enable/disable)
- Server-side validation: URL reachability, cron format, mapping syntax, at least one required selector
- Angular UI for source list and edit form
- Interactive mapping "designer":
  - User pastes a source URL
  - UI fetches and renders a sample response (feed items or HTML page)
  - User clicks on fields in the sample to build the mapping visually
  - Live preview of parsed items against the current mapping
- Hot reload of Hangfire jobs when sources change (no app restart)
- Test endpoint: trigger a one-off crawl and return parsed items for debugging

## NotificationService

### Complete webhooks API

Currently only `POST /api/v1/webhooks` is implemented. The original contract in
`docs/Solution/Solution.md` expects a full CRUD API.

**Scope:**
- `GET /api/v1/webhooks` — list with default pager (OrderBy, OrderDirection, Offset, Take)
- `GET /api/v1/webhooks/{id}` — single webhook
- `PUT /api/v1/webhooks/{id}` — update URL / EventType / enabled flag
- `DELETE /api/v1/webhooks/{id}` — unsubscribe
- Unit tests for BLL and DAL

### SignalR realtime notifications

Add SignalR hub to NotificationService to push realtime events to the frontend
(new news published, webhook delivered/failed, etc.).

**Scope:**
- SignalR hub in NotificationService.API (`/hubs/notifications`)
- Consume `webhook-triggered` queue → broadcast to connected clients
- Angular client: `@microsoft/signalr` integration, in-app toast on new news
- Authentication for hub connections (once Authorization Service is ready)

## Authorization Service

New microservice for user authentication and authorization across the platform.

**Scope:**
- New `.NET 9` project following the existing layered architecture (API / BLL / DAL)
- Identity storage: PostgreSQL via EF Core (shared instance or dedicated DB)
- JWT issuance + refresh tokens
- Endpoints: register, login, refresh, logout, me, password reset
- Shared `Common.Auth` library for JWT validation middleware consumed by
  NewsService / NotificationService / CrawlerService
- Angular client: login/register pages, auth guard, HTTP interceptor for Bearer token
- Integration with News rating, Comments, Webhooks (tie user identity to actions)

**Considerations:**
- Decide: build from scratch vs. adopt Duende IdentityServer / OpenIddict / Keycloak
- Role/claim model: simple (`User`, `Admin`) or policy-based
- Where to store refresh tokens (DB vs. distributed cache)

## Frontend

### Realtime notifications (SignalR)

Integrate SignalR client once the NotificationService hub is up. Toast notification
when new news arrives, subtle unread counter in nav bar.

### UI polish

General cleanup of the client: consistent spacing/typography, loading states, empty
states, error handling, mobile responsiveness.

## Testing

### NewsService integration tests

Add `NewsService.DAL.IntegrationTests` project mirroring CrawlerService integration
tests (Testcontainers for PostgreSQL). Cover `NewsStore`, `CommentStore` CRUD and
query paths.

### NotificationService tests

No tests exist for NotificationService today.

**Scope:**
- Unit tests for BLL (`WebhookDispatcher`, `WebhooksProcessor`, `WebhookTriggeredProcessor`,
  `WebhookDeliveryService`)
- DAL integration tests for `WebhookSubscriptionStore` (Testcontainers MongoDB)
- API tests for `WebhooksController`

## Infrastructure

### CI/CD pipeline

`.github/workflows/` is empty today. Set up automated build, test and (optionally) deploy.

**Scope:**
- `ci.yml` — on PR and push to `master`: restore → build all solutions → run unit
  tests → run integration tests (Testcontainers needs Docker runner)
- Build matrix: `NewsService.sln`, `CrawlerService.sln`, `NotificationService.sln`
- Angular build + unit tests (Vitest)
- Code coverage report upload
- `deploy.yml` (stretch) — build Docker images per service, push to registry,
  deploy via docker-compose on target host
- Branch protection: require green CI before merge

### Dev-experience: Claude Code Skills / Hooks / Subagents / Rules

Explore and adopt Claude Code automation primitives to speed up recurring work.

**Scope:**
- **Skills**: audit `.claude/skills/` (already have `create-migration`, `create-unit-test`,
  `up-to-date-tests`). Identify new candidates: `create-integration-test` (finish the
  placeholder), `add-source` (scaffold a new NewsSource in Mongo populator), `new-service`
  (scaffold API/BLL/DAL layered project).
- **Hooks**: decide policies for pre-commit / pre-tool-use hooks (e.g., auto-run
  `dotnet format`, block commits touching appsettings with secrets).
- **Subagents**: evaluate specialized agents (e.g., code-reviewer, test-writer) for
  review-style tasks.
- **Rules / CLAUDE.md**: keep root CLAUDE.md lean; consider per-service
  `CLAUDE.md` files for service-specific conventions.

### Migrate logging from Seq to Grafana Loki

Currently all services ship logs to Seq (single-node, `http://localhost:5341`) via Serilog.
Seq is convenient for dev, but as the stack grows consider migrating to Grafana Loki +
Promtail for better integration with metrics/traces and long-term retention.

**Scope:**
- Run Loki + Promtail + Grafana in docker-compose alongside the existing stack
- Add `Serilog.Sinks.Grafana.Loki` to the shared `Logger` project or keep both sinks
  behind configuration
- Structured fields: `ServiceName`, `SourceId`, `CorrelationId` — propagate correlation
  id through RabbitMQ headers and restore on consumer side
- Dashboards: errors per service, crawl latency, news throughput
- Document connection settings and default credentials in `readme.md`
