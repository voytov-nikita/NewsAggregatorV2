using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.Models.Models;
using MessageQueue.Abstractions;
using MessageQueue.Models;

namespace CrawlerService.BLL.Services;

internal class NewsQueueService : INewsQueueService
{
    private readonly INewsProducer _producer;

    public NewsQueueService(INewsProducer producer)
    {
        _producer = producer;
    }

    public async Task AddManyToQueueAsync(ParsedNews[] data)
    {
        if (data.Length == 0)
        {
            Console.WriteLine("No news to add to the queue.");
            return;
        }
        Console.WriteLine("Adding to the queue");
        Console.WriteLine($"Count: {data.Length}");
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
            Guid = x.Guid,
        }).ToArray();

        _producer.Publish(a);

        Console.WriteLine("Adding complete");
        Console.WriteLine("``````````````````````");
    }
}