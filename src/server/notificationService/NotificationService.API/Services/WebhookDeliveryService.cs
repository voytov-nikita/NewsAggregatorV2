using NotificationService.BLL.Abstractions.Services;
using NotificationService.Models.Webhooks;

namespace NotificationService.Services;

public class WebhookDeliveryService : IWebhookDeliveryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebhookDeliveryAttemptService _deliveryAttemptService;

    public WebhookDeliveryService(IHttpClientFactory httpClientFactory, IWebhookDeliveryAttemptService deliveryAttemptService)
    {
        _httpClientFactory = httpClientFactory;
        _deliveryAttemptService = deliveryAttemptService;
    }

    public async Task DeliverAsync(WebhookDispatchModel model)
    {
        var client = _httpClientFactory.CreateClient();

        var payload = new WebhookPayload
        {
            Id = Guid.NewGuid(),
            EventType = model.EventType,
            SubscriptionId = model.SubscriptionId,
            TimeStamp = DateTime.UtcNow,
            Data = model.Payload
        };

        var response = await client.PostAsJsonAsync(model.Url, payload);

        var deliveryAttempt = new WebhookDeliveryAttemptModel
        {
            Id = Guid.NewGuid().ToString(),
            SubscriptionId = model.SubscriptionId,
            Payload = model.Payload,
            ResponseStatusCode = (int)response.StatusCode,
            ResponseMessage = await response.Content.ReadAsStringAsync(),
            Timestamp = DateTime.UtcNow
        };

        await _deliveryAttemptService.AddAsync(deliveryAttempt);
    }

    public async Task DeliverManyAsync(IEnumerable<WebhookDispatchModel> models)
    {
        var tasks = models.Select(DeliverAsync);
        await Task.WhenAll(tasks);
    }
}

public interface IWebhookDeliveryService
{
    Task DeliverAsync(WebhookDispatchModel model);
    Task DeliverManyAsync(IEnumerable<WebhookDispatchModel> models);
}
