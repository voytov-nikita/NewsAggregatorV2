namespace NotificationService.DAL.Abstractions.Stores;

public interface IWebhooksStore
{
    public Task AddAsync(string url, string action);
}