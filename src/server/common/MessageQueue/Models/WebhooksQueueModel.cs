namespace MessageQueue.Models;

public class WebhooksQueueModel
{
    public string EventType { get; set; }
    public object? Data { get; set; }
}