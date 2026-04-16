using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Abstractions.Services;

public interface ISourceCrawler
{
    SourceType Type { get; }
    Task<ParsedNews[]> CrawlAsync(NewsSource source);
}
