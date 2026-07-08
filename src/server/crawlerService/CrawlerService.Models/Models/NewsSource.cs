using Common.Models;
using CrawlerService.Models.Enums;

namespace CrawlerService.Models.Models;

public class NewsSource
{
    /// <summary>Business slug used as Mongo _id (e.g. "pravda", "bbc-ua").</summary>
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string PublisherName { get; set; } = null!;
    public string PublisherLink { get; set; } = null!;
    public string Url { get; set; } = null!;

    public SourceType Type { get; set; }

    /// <summary>6-field Hangfire cron expression (with seconds).</summary>
    public string CronSchedule { get; set; } = null!;

    /// <summary>Optional charset name (e.g. "windows-1251"). Null = UTF-8.</summary>
    public string? Encoding { get; set; }

    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Category assigned to every article from this source. Set by the admin
    /// when configuring the source; defaults to <see cref="NewsCategory.Uncategorized"/>.
    /// </summary>
    public NewsCategory Category { get; set; } = NewsCategory.Uncategorized;

    public FeedFieldMapping? FeedMapping { get; set; }
    public HtmlSelectors? HtmlSelectors { get; set; }

    // ---- Runtime crawl metrics (updated by NewsDownloaderService / NewsService) ----

    public DateTime? LastCrawlAt { get; set; }
    public bool? LastCrawlSuccess { get; set; }
    public int LastCrawlDurationMs { get; set; }
    public int LastParsedCount { get; set; }

    /// <summary>Cumulative count of unique inserts credited to this source.</summary>
    public int ArticlesCount { get; set; }

    public int ConsecutiveFailures { get; set; }
}
