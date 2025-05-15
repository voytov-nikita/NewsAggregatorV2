using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsService.DAL.Abstractions.Stores;
using NewsService.DAL.PostgreSql.Filters;
using NewsService.DAL.PostgreSql.Stores;

namespace NewsService.DAL.PostgreSql;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<NewsServiceDbContext>(builder => builder
            .UseNpgsql(connectionString, optionsBuilder => optionsBuilder.EnableRetryOnFailure())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .EnableSensitiveDataLogging());
        
        services.AddTransient<IStartupFilter, MigrationsFilter>();
            
        services.AddTransient<INewsStore, NewsStore>();
        services.AddTransient<ICommentsStore, CommentsStore>();

        return services;
    }
}