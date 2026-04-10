using MessageQueue.Abstractions;
using MessageQueue.Models;

namespace MessageQueue.Services;

public class WebhookDispatcher : IWebhookDispatcher
{
    private readonly IWebhooksProducer _producer;

    public WebhookDispatcher(IWebhooksProducer producer)
    {
        _producer = producer;
    }

    public void Dispatch<T>(string eventType, T data)
    {
        var message = new WebhooksQueueModel
        {
            EventType = eventType,
            Data = data
        };

        _producer.Publish(message);
    }
}
