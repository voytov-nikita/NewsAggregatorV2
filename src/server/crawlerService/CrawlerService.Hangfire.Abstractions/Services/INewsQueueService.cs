using CrawlerService.Hangfire.Models;

namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsQueueService
{
    Task AddManyToQueueAsync(RawNews[] data);
}