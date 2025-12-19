namespace NotificationService.BLL.Abstractions.Services;

public interface IWebhooksServices
{
    public Task AddAsync(string url, string action);
}