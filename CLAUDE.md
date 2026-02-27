# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

News aggregator with a microservices backend (.NET 9 / C#) and an Angular 21 frontend. Three backend services communicate via RabbitMQ message queues:

- **NewsService** — Main API for aggregated news and comments. PostgreSQL via EF Core. Consumes `news-queue` (from CrawlerService), produces to `webhooks-queue` (for NotificationService).
- **CrawlerService** — Scheduled RSS crawler. MongoDB. Uses Hangfire for job scheduling. Produces to `news-queue`.
- **NotificationService** — Webhook/notification delivery. MongoDB. Consumes `webhooks-queue`.
- **NewsAggregatorClient** — Angular 21 SPA frontend.

## Repository Layout

```
src/server/common/          Shared libraries (Common, Common.API, Logger, MessageQueue)
src/server/newsService/     NewsService.API / .BLL / .DAL.PostgreSql / .Models
src/server/crawlerService/  CrawlerService.API / .BLL / .DAL / .Hangfire / .Models
src/server/notificationService/  NotificationService.API / .BLL / .DAL / .Models
src/client/NewsAggregatorClient/ Angular frontend
tests/newsService/          NewsService.BLL.Tests
tests/crawlerService/       API.Tests, BLL.Tests, DAL.IntegrationTests
docs/Solution/              Architecture docs and diagrams
```

Each backend service has its own solution file at the repo root: `NewsService.sln`, `CrawlerService.sln`, `NotificationService.sln`.

## Build & Run Commands

### Backend (.NET)

```bash
# Build a specific service
dotnet build NewsService.sln
dotnet build CrawlerService.sln
dotnet build NotificationService.sln

# Run a service (from its API directory)
dotnet run --project src/server/newsService/NewsService.API
dotnet run --project src/server/crawlerService/CrawlerService.API
dotnet run --project src/server/notificationService/NotificationService.API
```

### Frontend (Angular)

```bash
cd src/client/NewsAggregatorClient
npm install
npm start          # dev server at localhost:4200
npm run build      # production build
npm test           # run tests (Vitest)
```

### Running Tests

```bash
# All tests for a service
dotnet test NewsService.sln
dotnet test CrawlerService.sln

# Single test project
dotnet test tests/newsService/NewsService.BLL.Tests
dotnet test tests/crawlerService/CrawlerService.BLL.Tests

# Single test by name
dotnet test tests/newsService/NewsService.BLL.Tests --filter "FullyQualifiedName~TestMethodName"
```

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

**Message queue abstraction**: `IMessageProducer` / `IMessageConsumer` in `Common/MessageQueue/` with Polly resilience policies. Queue names: `news-queue`, `webhooks-queue`.

**Testing stack**: xUnit + Moq + AutoFixture + FluentAssertions. Angular uses Vitest + Jsdom.

## Angular Client Structure

NgModule-based architecture (not standalone components). Feature modules: `NewsModule`, `SettingsModule`, `LayoutModule`, `SharedModule`. Shared models/enums/services live in `src/app/shared/`. Prettier is configured in `package.json` (100 char width, single quotes, angular HTML parser).
