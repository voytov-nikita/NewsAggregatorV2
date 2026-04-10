using MessageQueue.Abstractions;
using MessageQueue.Models;
using MessageQueue.Settings;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace MessageQueue.Services;

public class WebhookTriggeredProducer : BaseMessageProducer<WebhookTriggeredQueueModel>, IWebhookTriggeredProducer
{
    public WebhookTriggeredProducer(MessageQueueSettings settings, ResiliencePipelineProvider<string> pipelineProvider, ILogger<WebhookTriggeredProducer> logger)
        : base(settings.ServerAddress, settings.QueueName, pipelineProvider, logger)
    {
    }
}
