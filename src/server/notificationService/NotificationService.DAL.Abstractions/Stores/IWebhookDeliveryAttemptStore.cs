using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Abstractions.Stores;

public interface IWebhookDeliveryAttemptStore
{
    Task AddAsync(WebhookDeliveryAttemptModel model);
}
