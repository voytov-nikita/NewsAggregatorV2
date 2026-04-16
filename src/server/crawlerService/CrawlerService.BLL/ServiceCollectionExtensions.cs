using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services;
using CrawlerService.BLL.Services.Crawlers;
using Microsoft.Extensions.DependencyInjection;

namespace CrawlerService.BLL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddTransient<INewsService, NewsService>()
            .AddTransient<INewsDownloaderService, NewsDownloaderService>()
            .AddTransient<INewsQueueService, NewsQueueService>();

        services.AddTransient<ISourceCrawler, FeedSourceCrawler>()
            .AddTransient<ISourceCrawler, HtmlSourceCrawler>()
            .AddTransient<ISourceCrawlerFactory, SourceCrawlerFactory>();

        return services;
    }
}
