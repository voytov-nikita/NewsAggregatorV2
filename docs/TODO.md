# TODO

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

### Centralized logging

Currently logs go only to the console via Serilog. Stand up a log aggregation stack
and wire all services to ship logs there.

**Under consideration:** Grafana Loki + Promtail — scales well and integrates with
Grafana dashboards if metrics are added later. Other options can be evaluated too.

**Scope:**
- Run in docker-compose alongside Postgres/Mongo/LavinMQ
- Configure Serilog sinks in all services
- Document connection settings in `readme.md`
