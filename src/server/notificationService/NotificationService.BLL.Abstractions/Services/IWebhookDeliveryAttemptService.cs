using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Abstractions.Services;

public interface IWebhookDeliveryAttemptService
{
    Task AddAsync(WebhookDeliveryAttemptModel model);
}
