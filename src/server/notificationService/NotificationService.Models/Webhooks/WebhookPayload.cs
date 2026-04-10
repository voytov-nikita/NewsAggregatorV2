namespace NotificationService.Models.Webhooks;

public class WebhookPayload
{
    public required Guid Id { get; set; }
    public required string EventType { get; set; }
    public required string SubscriptionId { get; set; }
    public DateTime TimeStamp { get; set; }
    public object? Data { get; set; }
}
