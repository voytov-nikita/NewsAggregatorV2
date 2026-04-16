# TODO

## NewsService

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

## Infrastructure

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
