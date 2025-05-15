using CrawlerService.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CrawlerService.BLL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddTransient<NewsService>();
        
        return services;
    }
}