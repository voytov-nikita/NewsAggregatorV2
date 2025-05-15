using MessageQueue.Abstractions;
using MessageQueue.Models;
using MessageQueue.Settings;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace MessageQueue.Services;

public class NewsConsumer: BaseMessageConsumer<NewsQueueModel[]>, INewsConsumer
{
    public NewsConsumer(MessageQueueSettings settings)
        : base(settings.ServerAddress, settings.QueueName)
    {
    }
}