using Microsoft.Extensions.DependencyInjection;
using NotificationService.BLL.Abstractions.Services;
using NotificationService.BLL.Services;

namespace NotificationService.BLL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddTransient<IWebhooksServices, WebhooksServices>();
        
        return services;
    }
}