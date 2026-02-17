using Common.Models;
using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Abstractions.Stores;

public interface IWebhookSubscriptionsStore
{
    public Task AddAsync(string url, string action);
    public Task<OffsetCollection<WebhookSubscriptionModel>> GetManyAsync(WebhooksFilter filter, OffsetPagination pagination);
}

public class WebhooksFilter
{
    public string[]? Actions { get; set; }
    public string[]? Ids { get; set; }
}