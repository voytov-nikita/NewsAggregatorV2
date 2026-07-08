using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using MongoDB.Driver;

namespace CrawlerService.DAL.Stores;

internal class SourceStore : ISourceStore
{
    internal const string CollectionName = "sources";
    private readonly IMongoDatabase _database;

    public SourceStore(IMongoDatabase database)
    {
        _database = database;
    }

    private IMongoCollection<NewsSource> Collection =>
        _database.GetCollection<NewsSource>(CollectionName);

    public async Task<NewsSource[]> GetAllAsync()
    {
        List<NewsSource> list = await Collection.Find(FilterDefinition<NewsSource>.Empty).ToListAsync();
        return list.ToArray();
    }

    public async Task<NewsSource[]> GetAllEnabledAsync()
    {
        List<NewsSource> list = await Collection.Find(s => s.Enabled).ToListAsync();
        return list.ToArray();
    }

    public async Task<NewsSource?> GetByIdAsync(string id)
    {
        return await Collection.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpsertAsync(NewsSource source)
    {
        await Collection.ReplaceOneAsync(
            s => s.Id == source.Id,
            source,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task<bool> DeleteAsync(string id)
    {
        DeleteResult result = await Collection.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task UpdateMetricsAsync(
        string id,
        DateTime lastAt,
        bool success,
        int durationMs,
        int parsedCount)
    {
        UpdateDefinitionBuilder<NewsSource> u = Builders<NewsSource>.Update;
        UpdateDefinition<NewsSource> update = u
            .Set(s => s.LastCrawlAt, lastAt)
            .Set(s => s.LastCrawlSuccess, success)
            .Set(s => s.LastCrawlDurationMs, durationMs)
            .Set(s => s.LastParsedCount, parsedCount);

        // Failure counter: reset on success, increment on failure.
        update = success
            ? update.Set(s => s.ConsecutiveFailures, 0)
            : update.Inc(s => s.ConsecutiveFailures, 1);

        await Collection.UpdateOneAsync(s => s.Id == id, update);
    }

    public async Task IncrementArticlesAsync(string id, int delta)
    {
        if (delta == 0) return;
        UpdateDefinition<NewsSource> update = Builders<NewsSource>.Update.Inc(s => s.ArticlesCount, delta);
        await Collection.UpdateOneAsync(s => s.Id == id, update);
    }
}
