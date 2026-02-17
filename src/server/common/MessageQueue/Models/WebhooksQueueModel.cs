namespace MessageQueue.Models;

public class WebhooksQueueModel
{
    public string Action { get; set; }
    public object? Data { get; set; }
}