using NotificationService.Models.Webhooks;

namespace NotificationService.Services;

public class WebhookDispatcher: IWebhookDispatcher
{
    private readonly IHttpClientFactory _httpClientFactory;
    public WebhookDispatcher(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task DispatchAsync(WebhookDispatchModel model)
    {
        var client = _httpClientFactory.CreateClient();

        var request = new
        {
            Id = Guid.NewGuid(),
            SubscriptionId = model.SubscriptionId,
            TimeStamp = DateTime.UtcNow,
            Data = model.Payload
        };

        var response = await client.PostAsJsonAsync(model.Url, request);
        //Todo
    }

    public async Task DispatchManyAsync(IEnumerable<WebhookDispatchModel> models)
    {
        var tasks = models.Select(DispatchAsync);
        await Task.WhenAll(tasks);
    }
}

public interface IWebhookDispatcher
{
    Task DispatchAsync(WebhookDispatchModel model);
    Task DispatchManyAsync(IEnumerable<WebhookDispatchModel> models);
}