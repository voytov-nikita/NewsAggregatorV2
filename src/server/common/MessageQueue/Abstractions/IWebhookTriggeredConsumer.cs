using MessageQueue.Models;

namespace MessageQueue.Abstractions;

public interface IWebhookTriggeredConsumer : IMessageConsumer<WebhookTriggeredQueueModel>
{
}
