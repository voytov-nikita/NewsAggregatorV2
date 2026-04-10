using MessageQueue.Abstractions;
using MessageQueue.Models;
using MessageQueue.Settings;

namespace MessageQueue.Services;

public class WebhookTriggeredConsumer : BaseMessageConsumer<WebhookTriggeredQueueModel>, IWebhookTriggeredConsumer
{
    public WebhookTriggeredConsumer(MessageQueueSettings settings)
        : base(settings.ServerAddress, settings.QueueName)
    {
    }
}
