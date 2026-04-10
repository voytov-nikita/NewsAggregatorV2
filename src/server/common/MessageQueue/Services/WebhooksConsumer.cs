using MessageQueue.Abstractions;
using MessageQueue.Models;
using MessageQueue.Settings;

namespace MessageQueue.Services;

public class WebhooksConsumer: BaseMessageConsumer<WebhooksQueueModel>, IWebhooksConsumer
{
    public WebhooksConsumer(MessageQueueSettings settings)
        : base(settings.ServerAddress, settings.QueueName)
    {
    }
}
