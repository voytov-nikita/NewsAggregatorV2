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

**Testing stack**: xUnit + Moq + AutoFixture + FluentAssertions. Angular uses Vitest + Jsdom.

## Angular Client

Angular 21 SPA, NgModule-based architecture. Details in [`src/client/NewsAggregatorClient/CLAUDE.md`](src/client/NewsAggregatorClient/CLAUDE.md).

## Available Skills

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
