using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Models.Models;
using MessageQueue.Abstractions;
using MessageQueue.Models;

namespace CrawlerService.Hangfire.Services;

class NewsQueueService : INewsQueueService
{
    private readonly INewsProducer _producer;

    public NewsQueueService(INewsProducer producer)
    {
        _producer = producer;
    }

    public async Task AddManyToQueueAsync(ParsedNews[] data)
    {
        Console.WriteLine("Adding to the queue");

        NewsQueueModel[] a = data.Select(x => new NewsQueueModel()
        {
            Title = x.Title,
            Description = x.Description,
            OriginalLink = x.OriginalLink,
            ImageLink = x.ImageLink,
            PublishDate = x.PublishDate,
            PublisherName = x.PublisherName,
            PublisherLink = x.PublisherLink,
            PublisherGuid = x.PublisherGuid,
            CompositeGuid = x.CompositeGuid,
        }).ToArray();
        
        _producer.Publish(a);
        
        Console.WriteLine("Adding complete");
    }
}