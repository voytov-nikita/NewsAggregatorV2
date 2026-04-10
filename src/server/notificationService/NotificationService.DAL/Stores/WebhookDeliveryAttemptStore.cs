using MongoDB.Driver;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.DAL.Entities;
using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Stores;

internal class WebhookDeliveryAttemptStore : IWebhookDeliveryAttemptStore
{
    internal const string CollectionName = "webhookDeliveryAttempts";
    private readonly IMongoDatabase _database;

    public WebhookDeliveryAttemptStore(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task AddAsync(WebhookDeliveryAttemptModel model)
    {
        var collection = _database.GetCollection<WebhookDeliveryAttemptEntity>(CollectionName);

        var entity = new WebhookDeliveryAttemptEntity
        {
            SubscriptionId = model.SubscriptionId,
            Payload = model.Payload,
            ResponseStatusCode = model.ResponseStatusCode,
            ResponseMessage = model.ResponseMessage,
            Timestamp = model.Timestamp
        };

        await collection.InsertOneAsync(entity);
    }
}
