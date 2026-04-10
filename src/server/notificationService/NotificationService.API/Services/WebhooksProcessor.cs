using Common.Models;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using NotificationService.DAL.Abstractions.Stores;

namespace NotificationService.Services;

public class WebhooksProcessor : BackgroundService
{
    private readonly IWebhooksConsumer _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WebhooksProcessor(IWebhooksConsumer consumer, IServiceScopeFactory serviceScopeFactory)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.AddSubscription(Process);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task Process(WebhooksQueueModel message)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            var webhookSubscriptionsStore = scope.ServiceProvider.GetRequiredService<IWebhookSubscriptionsStore>();
            WebhooksFilter filter = new WebhooksFilter()
            {
                Actions = [message.EventType],
            };
            var webhookSubscriptions = await webhookSubscriptionsStore.GetManyAsync(filter, OffsetPagination.None);

            var webhookTriggeredProducer = scope.ServiceProvider.GetRequiredService<IWebhookTriggeredProducer>();

            foreach (var subscription in webhookSubscriptions)
            {
                webhookTriggeredProducer.Publish(new WebhookTriggeredQueueModel
                {
                    Url = subscription.Url,
                    SubscriptionId = subscription.Id,
                    EventType = message.EventType,
                    Data = message.Data
                });
            }
        }
    }
}
