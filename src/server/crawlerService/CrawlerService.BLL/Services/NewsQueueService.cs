using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.Models.Models;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services;

internal class NewsQueueService : INewsQueueService
{
    private readonly INewsProducer _producer;
    private readonly ILogger<NewsQueueService> _logger;

    public NewsQueueService(INewsProducer producer, ILogger<NewsQueueService> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public Task AddManyToQueueAsync(ParsedNews[] data)
    {
        if (data.Length == 0)
        {
            _logger.LogInformation("No news to add to the queue");
            return Task.CompletedTask;
        }

        NewsQueueModel[] messages = data.Select(x => new NewsQueueModel
        {
            Title = x.Title,
            Description = x.Description,
            OriginalLink = x.OriginalLink,
            ImageLink = x.ImageLink,
            PublishDate = x.PublishDate,
            PublisherName = x.PublisherName,
            PublisherLink = x.PublisherLink,
            PublisherGuid = x.PublisherGuid,
            Guid = x.Guid,
            Category = x.Category,
        }).ToArray();

        _producer.Publish(messages);

        _logger.LogInformation("Published {Count} news messages to the queue", messages.Length);
        return Task.CompletedTask;
    }
}
