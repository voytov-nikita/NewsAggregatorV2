using MessageQueue.Models;

namespace MessageQueue.Abstractions;

public interface IWebhooksConsumer: IMessageConsumer<WebhooksQueueModel[]>
{

}