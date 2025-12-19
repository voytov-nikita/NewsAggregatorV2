using MongoDB.Driver;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.DAL.Entities;

namespace NotificationService.DAL.Stores;

internal class WebhooksStore: IWebhooksStore
{
    internal const string WebhooksCollectionName = "webhooks";
    private readonly IMongoDatabase _database;

    public WebhooksStore(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task AddAsync(string url, string action)
    {
        var collection = _database.GetCollection<WebhookEntity>(WebhooksCollectionName);

        WebhookEntity lastReadNewsEntities = new WebhookEntity
        {
            Url = url,
            Action = action,
            CreationTime = DateTime.UtcNow
        };

        await collection.InsertOneAsync(lastReadNewsEntities);
    }
}