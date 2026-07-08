using Common.Models;
using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Abstractions.Stores;

public interface IWebhookSubscriptionsStore
{
    Task<string> AddAsync(string url, string action);
    Task<WebhookSubscriptionModel?> GetByIdAsync(string id);
    Task<OffsetCollection<WebhookSubscriptionModel>> GetManyAsync(WebhooksFilter filter, OffsetPagination pagination);
    Task<bool> UpdateAsync(string id, string url, string action, bool enabled);
    Task<bool> DeleteAsync(string id);
}

public class WebhooksFilter
{
    public string[]? Actions { get; set; }
    public string[]? Ids { get; set; }
    public bool? Enabled { get; set; }
}
