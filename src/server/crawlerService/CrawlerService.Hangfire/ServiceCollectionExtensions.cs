using CrawlerService.Hangfire.Services;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace CrawlerService.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireCustomSettings(this IServiceCollection collection, HangfireSettings settings)
    {
        collection.AddHangfire(configuration => configuration.UseMongoStorage(settings.ConnectionString))
            .AddHangfireServer();
        
        collection.AddTransient<NewsDownloaderService>();
        collection.AddHostedService<HangfireSchedulerBackgroundService>();
        
        return collection;
    }
}

public class HangfireSettings
{
    public string ConnectionString { get; set; }
}