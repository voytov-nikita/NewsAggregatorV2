using Common.Models;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

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

    private async Task Process(WebhooksQueueModel[] messageModel)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            var _store = scope.ServiceProvider.GetRequiredService<IWebhookSubscriptionsStore>();
            WebhooksFilter filter = new WebhooksFilter()
            {
                Actions = messageModel.Select(x => x.Action).ToArray(),
            };
            var webhooks = await _store.GetManyAsync(filter, OffsetPagination.None);
            
            Console.WriteLine("Save new ");
            
            var dispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcher>();
            
            var dispatchModels = webhooks.Select(w => new WebhookDispatchModel
            {
                Url = w.Url,
                SubscriptionId = w.Id,
                Payload = messageModel.FirstOrDefault(m => m.Action == w.Action)?.Data
            });
            
            Console.WriteLine("Dispatch Action: " + string.Join(", ", dispatchModels.Select(_ => _.Url)));
            
            await dispatcher.DispatchManyAsync(dispatchModels);

        }
    }
}