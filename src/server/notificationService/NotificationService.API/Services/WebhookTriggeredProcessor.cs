using MessageQueue.Abstractions;
using MessageQueue.Models;
using NotificationService.Models.Webhooks;

namespace NotificationService.Services;

public class WebhookTriggeredProcessor : BackgroundService
{
    private readonly IWebhookTriggeredConsumer _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<WebhookTriggeredProcessor> _logger;

    public WebhookTriggeredProcessor(
        IWebhookTriggeredConsumer consumer,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<WebhookTriggeredProcessor> logger)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.AddSubscription(Process);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to subscribe to webhook-triggered queue");
            throw;
        }

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task Process(WebhookTriggeredQueueModel message)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var deliveryService = scope.ServiceProvider.GetRequiredService<IWebhookDeliveryService>();

        var dispatchModel = new WebhookDispatchModel
        {
            Url = message.Url,
            SubscriptionId = message.SubscriptionId,
            EventType = message.EventType,
            Payload = message.Data
        };

        await deliveryService.DeliverAsync(dispatchModel);
    }
}
