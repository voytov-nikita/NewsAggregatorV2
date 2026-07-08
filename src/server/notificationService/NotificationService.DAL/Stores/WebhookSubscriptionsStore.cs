using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Mappers;
using NotificationService.Models.Webhooks;

namespace NotificationService.DAL.Stores;

internal class WebhookSubscriptionsStore : IWebhookSubscriptionsStore
{
    internal const string WebhooksCollectionName = "webhooks";
    private readonly IMongoDatabase _database;

    public WebhookSubscriptionsStore(IMongoDatabase database)
    {
        _database = database;
    }

    private IMongoCollection<WebhookSubscriptionEntity> Collection =>
        _database.GetCollection<WebhookSubscriptionEntity>(WebhooksCollectionName);

    public async Task<string> AddAsync(string url, string action)
    {
        WebhookSubscriptionEntity entity = new WebhookSubscriptionEntity
        {
            Url = url,
            Action = action,
            CreationTime = DateTime.UtcNow,
            Enabled = true,
        };

        await Collection.InsertOneAsync(entity);
        return entity.Id.ToString();
    }

    public async Task<WebhookSubscriptionModel?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

        WebhookSubscriptionEntity? entity = await Collection
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync();

        return entity is null ? null : WebhookMapper.ToModel(entity);
    }

    public async Task<OffsetCollection<WebhookSubscriptionModel>> GetManyAsync(
        WebhooksFilter filter,
        OffsetPagination pagination)
    {
        FilterDefinition<WebhookSubscriptionEntity> filterDefinition = BuildFilter(filter);

        long totalCount = await Collection.CountDocumentsAsync(filterDefinition);

        List<WebhookSubscriptionEntity> entities = await Collection
            .Find(filterDefinition)
            .SortByDescending(x => x.CreationTime)
            .Skip(pagination.Offset)
            .Limit(pagination.Take)
            .ToListAsync();

        List<WebhookSubscriptionModel> models = entities.Select(WebhookMapper.ToModel).ToList();
        return new OffsetCollection<WebhookSubscriptionModel>(models, pagination.Offset, (int)totalCount);
    }

    public async Task<bool> UpdateAsync(string id, string url, string action, bool enabled)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId)) return false;

        UpdateDefinition<WebhookSubscriptionEntity> update = Builders<WebhookSubscriptionEntity>.Update
            .Set(x => x.Url, url)
            .Set(x => x.Action, action)
            .Set(x => x.Enabled, enabled);

        UpdateResult result = await Collection.UpdateOneAsync(x => x.Id == objectId, update);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId)) return false;

        DeleteResult result = await Collection.DeleteOneAsync(x => x.Id == objectId);
        return result.DeletedCount > 0;
    }

    private static FilterDefinition<WebhookSubscriptionEntity> BuildFilter(WebhooksFilter filter)
    {
        FilterDefinitionBuilder<WebhookSubscriptionEntity> builder = Builders<WebhookSubscriptionEntity>.Filter;
        FilterDefinition<WebhookSubscriptionEntity> filterDefinition = builder.Empty;

        if (filter.Actions is { Length: > 0 })
        {
            filterDefinition &= builder.In(x => x.Action, filter.Actions);
        }

        if (filter.Ids is { Length: > 0 })
        {
            ObjectId[] objectIds = filter.Ids.Select(ObjectId.Parse).ToArray();
            filterDefinition &= builder.In(x => x.Id, objectIds);
        }

        if (filter.Enabled.HasValue)
        {
            filterDefinition &= builder.Eq(x => x.Enabled, filter.Enabled.Value);
        }

        return filterDefinition;
    }
}
