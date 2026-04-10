namespace NotificationService.Models.Webhooks;

public class WebhookDeliveryAttemptModel
{
    public required string Id { get; set; }
    public required string SubscriptionId { get; set; }
    public object? Payload { get; set; }
    public int ResponseStatusCode { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime Timestamp { get; set; }
}
