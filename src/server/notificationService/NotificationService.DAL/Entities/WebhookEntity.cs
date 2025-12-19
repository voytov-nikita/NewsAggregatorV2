using MongoDB.Bson;

namespace NotificationService.DAL.Entities;

public class WebhookEntity
{
    public ObjectId Id { get; set; }
    public string Url { get; set; }
    public string Action { get; set; }
    public DateTime CreationTime { get; set; }
    
}