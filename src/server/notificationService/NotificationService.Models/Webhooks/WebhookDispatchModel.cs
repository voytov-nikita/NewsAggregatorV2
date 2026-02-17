namespace NotificationService.Models.Webhooks;

public class WebhookDispatchModel
{
    public required string Url { get; set; }
    public required string SubscriptionId { get; set; }
    public object? Payload { get; set; }
}
