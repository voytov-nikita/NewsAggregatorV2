
using Microsoft.Extensions.DependencyInjection;
using NewsService.BLL.Abstractions;
using NewsService.BLL.Abstractions.Services;
using NewsService.BLL.Abstractions.Validators;
using NewsService.BLL.Services;
using NewsService.BLL.Validators;

namespace NewsService.BLL;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddTransient<INewsService, Services.NewsService>();
        services.AddTransient<ICommentsService, CommentsService>();
        services.AddTransient<INewsVotesService, NewsVotesService>();
        
        services.AddTransient<INewsValidator, NewsValidator>();
        services.AddTransient<ICommentsValidator, CommentsValidator>();
        
        return services;
    }
}