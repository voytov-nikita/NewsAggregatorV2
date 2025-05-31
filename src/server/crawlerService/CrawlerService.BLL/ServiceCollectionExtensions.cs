using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CrawlerService.BLL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddTransient<INewsService, NewsService>()

            .AddTransient<INewsDownloaderService, NewsDownloaderService>()
            .AddTransient<INewsParserService, NewsParserService>()
            .AddTransient<INewsQueueService, NewsQueueService>();

        return services;
    }
}