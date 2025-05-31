using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.DAL.Entities;
using CrawlerService.Models.Models;
using MongoDB.Driver;

namespace CrawlerService.DAL.Stores;

internal class NewsStore : INewsStore
{
    internal const string RawNewsCollectionName = "rawNews";
    private readonly IMongoDatabase _database;

    public NewsStore(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task InsertAsync(ParsedNews[] parsedNews)
    {
        var collection = _database.GetCollection<RawNewsEntity>(RawNewsCollectionName);

        RawNewsEntity[] lastReadNewsEntities = parsedNews.Select(_ => new RawNewsEntity
        {
            GlobalUniqueId = _.GlobalUniqueId,
        }).ToArray();

        await collection.InsertManyAsync(lastReadNewsEntities);
    }

    public async Task<string[]> ExcludeExistedAsync(string[] globalUniqueIds)
    {
        List<RawNewsEntity> res = await _database.GetCollection<RawNewsEntity>(RawNewsCollectionName)
            .Find(_ =>
                globalUniqueIds.Contains(_.GlobalUniqueId)
            ).ToListAsync();

        return globalUniqueIds
            .Except(res.Select(_ => _.GlobalUniqueId))
            .ToArray();
    }
}