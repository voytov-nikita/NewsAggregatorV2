using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Abstractions.Services;

public interface INewsQueueService
{
    Task AddManyToQueueAsync(ParsedNews[] data);
}