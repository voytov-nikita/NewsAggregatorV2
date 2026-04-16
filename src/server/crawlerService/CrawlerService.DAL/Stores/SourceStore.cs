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
}
