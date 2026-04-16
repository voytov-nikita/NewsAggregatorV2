using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.Models.Enums;

namespace CrawlerService.BLL.Services;

internal class SourceCrawlerFactory : ISourceCrawlerFactory
{
    private readonly IReadOnlyDictionary<SourceType, ISourceCrawler> _crawlers;

    public SourceCrawlerFactory(IEnumerable<ISourceCrawler> crawlers)
    {
        _crawlers = crawlers.ToDictionary(c => c.Type);
    }

    public ISourceCrawler Get(SourceType type)
    {
        if (!_crawlers.TryGetValue(type, out ISourceCrawler? crawler))
        {
            throw new InvalidOperationException($"No crawler registered for source type '{type}'");
        }
        return crawler;
    }
}
