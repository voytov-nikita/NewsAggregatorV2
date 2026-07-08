using MongoDB.Bson;

namespace NotificationService.DAL.Entities;

public class WebhookSubscriptionEntity
{
    public ObjectId Id { get; set; }
    public string Url { get; set; }
    public string Action { get; set; }
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// When false the subscription is retained but skipped during fan-out.
    /// Defaults to true so existing rows read back as enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
