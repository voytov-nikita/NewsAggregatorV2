namespace NotificationService.BLL.Abstractions.Services;

public interface IWebhookSubscriptionsServices
{
    public Task AddAsync(string url, string eventType);
}