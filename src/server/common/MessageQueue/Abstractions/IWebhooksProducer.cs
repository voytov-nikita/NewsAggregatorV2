using MessageQueue.Models;

namespace MessageQueue.Abstractions;

public interface IWebhooksProducer: IMessageProducer<WebhooksQueueModel[]>
{

}
