using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Hangfire.Models;

namespace CrawlerService.Hangfire.Services;

class NewsQueueService : INewsQueueService
{
    public Task AddManyToQueueAsync(RawNews[] data)
    {
        throw new NotImplementedException();
    }
}