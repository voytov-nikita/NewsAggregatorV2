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
