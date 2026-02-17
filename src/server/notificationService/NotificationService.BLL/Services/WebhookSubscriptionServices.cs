using NotificationService.BLL.Abstractions.Services;
using NotificationService.DAL.Abstractions.Stores;

namespace NotificationService.BLL.Services;

public class WebhookSubscriptionServices: IWebhookSubscriptionsServices
{
    private readonly IWebhookSubscriptionsStore _store;

    public WebhookSubscriptionServices(IWebhookSubscriptionsStore store)
    {
        _store = store;
    }
    
    public async Task AddAsync(string url, string eventType)
    {
        await _store.AddAsync(url, eventType);
    }
}