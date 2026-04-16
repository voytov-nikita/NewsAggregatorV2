using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services;

public class NewsDownloaderService : INewsDownloaderService
{
    private readonly ISourceStore _sourceStore;
    private readonly ISourceCrawlerFactory _crawlerFactory;
    private readonly IPostponedJobRunner _postponedJobRunner;
    private readonly ILogger<NewsDownloaderService> _logger;

    public NewsDownloaderService(
        ISourceStore sourceStore,
        ISourceCrawlerFactory crawlerFactory,
        IPostponedJobRunner postponedJobRunner,
        ILogger<NewsDownloaderService> logger)
    {
        _sourceStore = sourceStore;
        _crawlerFactory = crawlerFactory;
        _postponedJobRunner = postponedJobRunner;
        _logger = logger;
    }

    public async Task GetNewsAsync(string sourceId)
    {
        NewsSource? source = await _sourceStore.GetByIdAsync(sourceId);
        if (source is null)
        {
            _logger.LogWarning("Source {SourceId} not found, skipping", sourceId);
            return;
        }

        if (!source.Enabled)
        {
            _logger.LogInformation("Source {SourceId} is disabled, skipping", sourceId);
            return;
        }

        ISourceCrawler crawler = _crawlerFactory.Get(source.Type);

        ParsedNews[] items = await crawler.CrawlAsync(source);
        if (items.Length == 0)
        {
            _logger.LogInformation("Source {SourceId}: no items to save", sourceId);
            return;
        }

        _postponedJobRunner.Enqueue<INewsService>(service => service.SaveUniqueNewsAsync(items));
    }
}
