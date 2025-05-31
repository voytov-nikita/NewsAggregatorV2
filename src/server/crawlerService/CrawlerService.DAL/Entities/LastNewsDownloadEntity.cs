using MongoDB.Bson;

namespace CrawlerService.DAL.Entities;

public class RawNewsEntity
{
    public ObjectId Id { get; set; }
    public string GlobalUniqueId { get; set; }
}