using MessageQueue.Models;

namespace MessageQueue.Abstractions;

public interface IWebhookTriggeredProducer : IMessageProducer<WebhookTriggeredQueueModel>
{
}
