namespace MessageQueue.Models;

public class WebhookTriggeredQueueModel
{
    public required string Url { get; set; }
    public required string SubscriptionId { get; set; }
    public string EventType { get; set; }
    public object? Data { get; set; }
}
