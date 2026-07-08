namespace NotificationService.Models.Webhooks;

public class WebhookSubscriptionModel
{
    public required string Id { get; set; }
    public required string Url { get; set; }
    public required string Action { get; set; }
    public DateTime CreationTime { get; set; }
    public bool Enabled { get; set; }
}
