
using Microsoft.Extensions.DependencyInjection;
using NewsService.BLL.Abstractions;
using NewsService.BLL.Services;

namespace NewsService.BLL;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddTransient<INewsService, Services.NewsService>();
        services.AddTransient<ICommentsService, CommentsService>();
        
        return services;
    }
}