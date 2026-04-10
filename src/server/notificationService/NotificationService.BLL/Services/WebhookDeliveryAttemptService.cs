using NotificationService.BLL.Abstractions.Services;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Services;

public class WebhookDeliveryAttemptService : IWebhookDeliveryAttemptService
{
    private readonly IWebhookDeliveryAttemptStore _store;

    public WebhookDeliveryAttemptService(IWebhookDeliveryAttemptStore store)
    {
        _store = store;
    }

    public async Task AddAsync(WebhookDeliveryAttemptModel model)
    {
        await _store.AddAsync(model);
    }
}
