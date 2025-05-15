using CrawlerService.Models.Models;

namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsQueueService
{
    Task AddManyToQueueAsync(ParsedNews[] data);
}