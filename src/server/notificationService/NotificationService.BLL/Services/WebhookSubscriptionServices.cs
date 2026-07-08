using Common.Models;
using NotificationService.BLL.Abstractions.Services;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Services;

public class WebhookSubscriptionServices : IWebhookSubscriptionsServices
{
    private readonly IWebhookSubscriptionsStore _store;

    public WebhookSubscriptionServices(IWebhookSubscriptionsStore store)
    {
        _store = store;
    }

    public Task<string> AddAsync(string url, string eventType) =>
        _store.AddAsync(url, eventType);

    public Task<WebhookSubscriptionModel?> GetByIdAsync(string id) =>
        _store.GetByIdAsync(id);

    public Task<OffsetCollection<WebhookSubscriptionModel>> GetManyAsync(
        WebhooksFilter filter,
        OffsetPagination pagination) =>
        _store.GetManyAsync(filter, pagination);

    public Task<bool> UpdateAsync(string id, string url, string eventType, bool enabled) =>
        _store.UpdateAsync(id, url, eventType, enabled);

    public Task<bool> DeleteAsync(string id) =>
        _store.DeleteAsync(id);
}
