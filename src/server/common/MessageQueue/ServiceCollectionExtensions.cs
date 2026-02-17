using MessageQueue.Abstractions;
using MessageQueue.Constants;
using MessageQueue.Services;
using MessageQueue.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;

namespace MessageQueue;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNewsServiceProducers(this IServiceCollection services, MessageQueueSettings settings)
    {
        services.AddSingleton(settings);

        AddResiliencePipelineRabbitMq(services);

        services.AddTransient<INewsProducer, NewsProducer>();

        return services;
    }

    public static IServiceCollection AddNewsServiceConsumers(this IServiceCollection services, MessageQueueSettings settings)
    {
        services.AddSingleton(settings);

        services.AddTransient<INewsConsumer, NewsConsumer>();

        return services;
    }


    public static IServiceCollection AddWebhooksConsumer(this IServiceCollection services, MessageQueueSettings settings)
    {
        services.AddSingleton(settings);

        services.AddTransient<IWebhooksConsumer, WebhooksConsumer>();

        return services;
    }

    public static IServiceCollection AddWebhooksProducer(this IServiceCollection services, MessageQueueSettings settings)
    {
        AddResiliencePipelineRabbitMq(services);

        services.AddTransient<IWebhooksProducer>(sp =>
        {
            var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();
            var logger = sp.GetRequiredService<ILogger<WebhooksProducer>>();
            return new WebhooksProducer(settings, pipelineProvider, logger);
        });

        return services;
    }

    public static void AddResiliencePipelineRabbitMq(this IServiceCollection services)
    {
        services.AddResiliencePipeline(MessageQueuePipelineConstants.ResiliencePipelineRabbitMq, configure =>
        {
            configure
                .AddRetry(new RetryStrategyOptions()
                {
                    ShouldHandle = new PredicateBuilder().Handle<BrokerUnreachableException>(),
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromSeconds(5)
                })
                .AddTimeout(TimeSpan.FromSeconds(30));
        });
    }
}