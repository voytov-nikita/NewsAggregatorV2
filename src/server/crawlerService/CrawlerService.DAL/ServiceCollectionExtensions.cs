using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.DAL.Populators;
using CrawlerService.DAL.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;

using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace CrawlerService.DAL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, DatabaseSettings settings)
    {
        MongoClientSettings clientSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.ConnectionString));

        services.AddSingleton<IMongoClient>(_ =>
        {
#if DEBUG
            clientSettings.LoggingSettings = new LoggingSettings(_.GetService<ILoggerFactory>());
#endif
            return new MongoClient(clientSettings);
        });
        services.AddTransient<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>()
            .GetDatabase(settings.DatabaseName));

        services.AddMigration(new MongoMigrationSettings()
        {
            ConnectionString = settings.ConnectionString,
            Database = settings.DatabaseName,
            //ClientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString)
        });

        services.AddTransient<INewsStore, NewsStore>();
        services.AddTransient<ISourceStore, SourceStore>();

        services.AddTransient<IPopulator, SourcePopulator>();

        return services;
    }
}

public class DatabaseSettings
{
    public string ConnectionString { get; init; } = null!;

    public string DatabaseName { get; init; } = null!;
}
