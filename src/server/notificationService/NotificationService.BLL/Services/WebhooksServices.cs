using NotificationService.BLL.Abstractions.Services;
using NotificationService.DAL.Abstractions.Stores;

namespace NotificationService.BLL.Services;

public class WebhooksServices: IWebhooksServices
{
    private readonly IWebhooksStore _store;

    public WebhooksServices(IWebhooksStore store)
    {
        _store = store;
    }
    
    public async Task AddAsync(string url, string action)
    {
        await _store.AddAsync(url, action);
    }
}