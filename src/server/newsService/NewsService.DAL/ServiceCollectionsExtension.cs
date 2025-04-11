using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsService.DAL.Abstractions;
using NewsService.DAL.Stores;

namespace NewsService.DAL;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
    {
        services.AddDbContext<NewsServiceDbContext>(builder => builder
            .UseNpgsql("TODO:AddConnectionString", optionsBuilder => optionsBuilder.EnableRetryOnFailure())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .EnableSensitiveDataLogging());

        services.AddTransient<INewsStore, NewsStore>();
        services.AddTransient<ICommentsStore, CommentsStore>();

        return services;
    }
}