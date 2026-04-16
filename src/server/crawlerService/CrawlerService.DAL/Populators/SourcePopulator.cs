using CrawlerService.DAL.Stores;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using MongoDB.Driver;

namespace CrawlerService.DAL.Populators;

internal class SourcePopulator : IPopulator
{
    public void Populate(IMongoDatabase database)
    {
        IMongoCollection<NewsSource> collection = database.GetCollection<NewsSource>(SourceStore.CollectionName);

        NewsSource[] seed =
        [
            new NewsSource
            {
                Id = "pravda",
                Name = "Ukrainska Pravda",
                PublisherName = "Pravda",
                PublisherLink = "https://www.pravda.com.ua/",
                Url = "https://www.pravda.com.ua/rss/",
                Type = SourceType.Feed,
                CronSchedule = "*/30 * * * * *",
                Enabled = true,
            },
        ];

        foreach (NewsSource source in seed)
        {
            bool exists = collection.Find(s => s.Id == source.Id).Any();
            if (!exists)
            {
                collection.InsertOne(source);
            }
        }
    }
}
