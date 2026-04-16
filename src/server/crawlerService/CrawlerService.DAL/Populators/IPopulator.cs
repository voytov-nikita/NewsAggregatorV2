using MongoDB.Driver;

namespace CrawlerService.DAL.Populators;

internal interface IPopulator
{
    void Populate(IMongoDatabase database);
}
