using CrawlerService.Models.Enums;

namespace CrawlerService.BLL.Abstractions.Services;

public interface ISourceCrawlerFactory
{
    ISourceCrawler Get(SourceType type);
}
