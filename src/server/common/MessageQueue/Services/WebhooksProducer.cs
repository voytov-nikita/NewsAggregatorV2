using MessageQueue.Abstractions;
using MessageQueue.Models;
using MessageQueue.Settings;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace MessageQueue.Services;

public class WebhooksProducer: BaseMessageProducer<WebhooksQueueModel[]>, IWebhooksProducer
{
    public WebhooksProducer(MessageQueueSettings settings, ResiliencePipelineProvider<string> pipelineProvider, ILogger<WebhooksProducer> logger)
        : base(settings.ServerAddress, settings.QueueName, pipelineProvider, logger)
    {
    }
}
