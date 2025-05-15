using MessageQueue.Abstractions;
using MessageQueue.Models;
using MessageQueue.Settings;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace MessageQueue.Services;

public class NewsProducer: BaseMessageProducer<NewsQueueModel[]>, INewsProducer
{
    public NewsProducer(MessageQueueSettings settings, ResiliencePipelineProvider<string> pipelineProvider, ILogger<NewsProducer> logger)
        : base(settings.ServerAddress, settings.QueueName, pipelineProvider, logger)
    {
    }
}