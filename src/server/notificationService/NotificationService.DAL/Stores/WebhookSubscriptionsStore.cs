using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Mappers;
using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Stores;

internal class WebhookSubscriptionsStore: IWebhookSubscriptionsStore
{
    internal const string WebhooksCollectionName = "webhooks";
    private readonly IMongoDatabase _database;

    public WebhookSubscriptionsStore(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task AddAsync(string url, string action)
    {
        var collection = _database.GetCollection<WebhookSubscriptionEntity>(WebhooksCollectionName);

        WebhookSubscriptionEntity lastReadNewsSubscriptionEntities = new WebhookSubscriptionEntity
        {
            Url = url,
            Action = action,
            CreationTime = DateTime.UtcNow
        };

        await collection.InsertOneAsync(lastReadNewsSubscriptionEntities);
    }

    public async Task<OffsetCollection<WebhookSubscriptionModel>> GetManyAsync(WebhooksFilter filter, OffsetPagination pagination)
    {
        var collection = _database.GetCollection<WebhookSubscriptionEntity>(WebhooksCollectionName);
        var filterDefinition = BuildFilter(filter);

        var totalCount = await collection.CountDocumentsAsync(filterDefinition);

        var entities = await collection.Find(filterDefinition)
            .Skip(pagination.Offset)
            .Limit(pagination.Take)
            .ToListAsync();

        var models = entities.Select(WebhookMapper.ToModel).ToList();

        return new OffsetCollection<WebhookSubscriptionModel>(models, pagination.Offset, (int)totalCount);
    }

    private static FilterDefinition<WebhookSubscriptionEntity> BuildFilter(WebhooksFilter filter)
    {
        var builder = Builders<WebhookSubscriptionEntity>.Filter;
        var filterDefinition = builder.Empty;

        if (filter.Actions is { Length: > 0 })
        {
            filterDefinition &= builder.In(x => x.Action, filter.Actions);
        }

        if (filter.Ids is { Length: > 0 })
        {
            var objectIds = filter.Ids.Select(ObjectId.Parse).ToArray();
            filterDefinition &= builder.In(x => x.Id, objectIds);
        }

        return filterDefinition;
    }
}