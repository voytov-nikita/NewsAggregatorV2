using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Hangfire.Services;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace CrawlerService.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireCustomSettings(this IServiceCollection collection,
        HangfireSettings settings)
    {
        collection.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseFilter(new AutomaticRetryAttribute { Attempts = 0 })
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(settings.ConnectionString,
                    new MongoStorageOptions
                    {
                        Prefix = settings.Prefix,
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy()
                        },
                        CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                    }))
            .AddHangfireServer();

        collection.AddTransient<INewsDownloaderService, NewsDownloaderService>();
        collection.AddTransient<INewsParserService, NewsParserService>();
        collection.AddTransient<INewsQueueService, NewsQueueService>();
        collection.AddTransient<INewsUniquenessService, NewsUniquenessService>();
        collection.AddTransient<INewsValidationService, NewsValidationService>();
        collection.AddHostedService<HangfireSchedulerBackgroundService>();

        return collection;
    }
}

public class HangfireSettings
{
    public string ConnectionString { get; set; }
    public string Prefix { get; set; }
}