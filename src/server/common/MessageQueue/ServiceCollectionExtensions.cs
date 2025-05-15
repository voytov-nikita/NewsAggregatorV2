using MessageQueue.Abstractions;
using MessageQueue.Constants;
using MessageQueue.Services;
using MessageQueue.Settings;
using Microsoft.Extensions.DependencyInjection;
using Polly;
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