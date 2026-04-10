using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.DAL.Entities;

public class WebhookDeliveryAttemptEntity
{
    public ObjectId Id { get; set; }
    public string SubscriptionId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public object? Payload { get; set; }
    public int ResponseStatusCode { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime Timestamp { get; set; }
}
