using CrawlerService.DAL.Entities;
using CrawlerService.Models.Models;
using MongoDB.Driver;

namespace CrawlerService.DAL.Stores;

public class NewsStore
{
    private const string LastReadNewsCollectionName = "lastReadNews";
    private readonly IMongoDatabase _database;

    public NewsStore(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task UpdateLastReadNewsAsync(ParsedNews[] parsedNews, string publisherName)
    {
        var collection = _database.GetCollection<LastReadNewsEntity>(LastReadNewsCollectionName);
        LastReadNewsEntity[] lastReadNewsEntities = parsedNews.Select(_ => new LastReadNewsEntity
        {
            Title = _.Title,
            Description = _.Description,
            OriginalLink = _.OriginalLink,
            ImageLink = _.ImageLink,
            PublishDate = _.PublishDate,
            PublisherName = _.PublisherName,
            PublisherLink = _.PublisherLink,
            PublisherGuid = _.PublisherGuid,
            CompositeGuid = _.CompositeGuid,
            Guid = _.Guid,
        }).ToArray();

        FilterDefinition<LastReadNewsEntity> filter = Builders<LastReadNewsEntity>.Filter.Where(x => x.PublisherName == publisherName);
        //Todo: Make it through transaction
        await collection.DeleteManyAsync(filter);
        await collection.InsertManyAsync(lastReadNewsEntities);
    }

    public async Task<List<ParsedNews>> GetLastReadNewsAsync(string[] customGuids)
    {
        var res = await _database.GetCollection<LastReadNewsEntity>(LastReadNewsCollectionName)
            .Find(_ =>
                customGuids.Contains(_.CompositeGuid)
            ).ToListAsync();
        
        //Todo: replace ParsedNews on LastReadNewsModel
        return res.Select(_ => new ParsedNews
        {
            Title = _.Title,
            Description = _.Description,
            OriginalLink = _.OriginalLink,
            ImageLink = _.ImageLink,
            PublishDate = _.PublishDate,
            PublisherName = _.PublisherName,
            PublisherLink = _.PublisherLink,
            PublisherGuid = _.PublisherGuid,
            CompositeGuid = _.CompositeGuid,
            Guid = _.Guid
        }).ToList();
    }
}