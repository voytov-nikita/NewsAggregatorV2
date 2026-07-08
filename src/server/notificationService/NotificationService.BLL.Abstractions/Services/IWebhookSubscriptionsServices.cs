using Common.Models;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Abstractions.Services;

public interface IWebhookSubscriptionsServices
{
    Task<string> AddAsync(string url, string eventType);
    Task<WebhookSubscriptionModel?> GetByIdAsync(string id);
    Task<OffsetCollection<WebhookSubscriptionModel>> GetManyAsync(WebhooksFilter filter, OffsetPagination pagination);
    Task<bool> UpdateAsync(string id, string url, string eventType, bool enabled);
    Task<bool> DeleteAsync(string id);
}
