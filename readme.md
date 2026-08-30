# News Aggregator

Microservices backend on .NET 9 + an Angular 21 SPA.

| Service | HTTPS port | Storage |
|---|---|---|
| NewsService | 7300 | PostgreSQL (`local-news`, schema `newsService`) |
| CrawlerService | 7310 | MongoDB (`news`) |
| NotificationService | 7320 | MongoDB (`notificationService`) |
| AuthService | 7330 | PostgreSQL (`local-news`, schema `authService`) |
| Angular client | 4200 (HTTP) | — |

---

# Prerequisites

- .NET SDK 9 or later
- Node.js 20+ and npm
- Docker
- `dotnet-ef` for creating migrations: `dotnet tool install --global dotnet-ef`

---

# 1. Infrastructure (Docker)

## PostgreSql
```
docker run -d --name postgres-news -p 5432:5432 --restart unless-stopped -e POSTGRES_PASSWORD=Qwerty123$% -v d:\news-data\postgresql:/var/lib/postgresql/data postgres:17
```
## Seq
```
docker run -d --name seq-news -p 5341:80 --restart unless-stopped -e ACCEPT_EULA=Y -v d:\news-data\seq:/data datalust/seq:latest
```
## Mongo
```
docker run -d --name mongodb-news -p 27017:27017 --restart unless-stopped -v d:\news-data\mongodb:/data/db mongo
```
## MessageQueue
```
docker run -d --name lavinmq-news -p 15672:15672 -p 5672:5672 -v d:\news-data\lavinmq:/tmp/amqp --restart unless-stopped cloudamqp/lavinmq
```

The `local-news` database, its schemas and its tables are created automatically: both PostgreSQL
services run EF Core migrations at startup through a `MigrationsFilter` (`IStartupFilter`). Nothing
has to be created by hand.

---

# 2. Trust the local HTTPS certificate — required

```
dotnet dev-certs https --trust
```

NewsService, CrawlerService and NotificationService validate access tokens by fetching AuthService's
JWKS over HTTPS on a back channel. Without a trusted certificate that fetch fails with a TLS error,
and **every authenticated request returns 500** — an error that looks nothing like an authorization
problem. If trusting the certificate is not an option on your machine, set
`"Auth": { "RequireHttpsMetadata": false }` in the consuming service's `appsettings.json`.

---

# 3. Seed the admin user (AuthService) — required for admin access

The seeded admin password is never committed. Put it in user secrets; without it `AdminUserSeeder`
logs a warning and skips, and the application starts with no admin account.

```
dotnet user-secrets set "Seed:AdminEmail" "admin@newsaggregator.local" --project src/server/authService/AuthService.API
dotnet user-secrets set "Seed:AdminPassword" "<your password>" --project src/server/authService/AuthService.API
```

The password must satisfy ASP.NET Core Identity's rules (at least 8 characters, upper- and lowercase,
a digit). Roles (`User`, `Admin`) and the six permissions are seeded from code on every start and need
no configuration.

> The launch profiles set `ASPNETCORE_ENVIRONMENT=Local`, which is **not** `Development`, so the
> default user-secrets registration would not apply. AuthService loads them explicitly for `Local`.

---

# 4. Token signing key (AuthService)

Nothing to do on first run: AuthService generates an RSA key at
`src/server/authService/AuthService.API/keys/signing-key.pem` and logs a warning that it did so. The
directory is gitignored, along with every `*.pem`.

**Do not delete that file.** The `kid` is derived from the key, so a regenerated key invalidates every
outstanding access token and every cached JWKS. To rotate deliberately, move the active key to
`keys/retired/<kid>.pem` and restart: retired keys stay published in JWKS while the new one signs, so
tokens issued earlier keep working until they expire.

---

# 5. Run the backend

Each service has its own solution at the repository root. Start AuthService first if you intend to
sign in, though the order does not actually matter — JWKS is fetched lazily on the first validation.

```
dotnet run --project src/server/authService/AuthService.API --launch-profile local
dotnet run --project src/server/newsService/NewsService.API --launch-profile local
dotnet run --project src/server/crawlerService/CrawlerService.API --launch-profile local
dotnet run --project src/server/notificationService/NotificationService.API --launch-profile local
```

Swagger is at `/swagger` on each service (for example `https://localhost:7330/swagger`). AuthService
publishes `/.well-known/openid-configuration` and `/.well-known/jwks.json`.

Logs go to the console and to Seq at `http://localhost:5341`.

---

# 6. Run the client

```
cd src/client/NewsAggregatorClient
npm install
npm start          # http://localhost:4200
```

The client calls NewsService and AuthService directly. AuthService's CORS policy enumerates the
allowed origins in `Cors:AllowedOrigins` (`http://localhost:4200` and `https://localhost:4200`)
because a browser rejects `AllowAnyOrigin()` alongside credentials, and the refresh cookie needs
credentials. Serving the client from a different origin means adding it to that list.

---

# 7. Build and test

```
dotnet build AuthService.sln;         dotnet test AuthService.sln
dotnet build NewsService.sln;         dotnet test NewsService.sln
dotnet build CrawlerService.sln;      dotnet test CrawlerService.sln
dotnet build NotificationService.sln; dotnet test NotificationService.sln

cd src/client/NewsAggregatorClient; npm test -- --watch=false
```

`npm test` alone stays in watch mode.

---

# 8. Creating migrations

```
dotnet ef migrations add <Name> --project src/server/newsService/NewsService.DAL.PostgreSql --startup-project src/server/newsService/NewsService.API
```

Migrations are applied automatically at startup, so read the generated `Up()` before starting the
service if the local database holds data worth keeping.
