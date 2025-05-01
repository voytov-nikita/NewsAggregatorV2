using CrawlerService.Hangfire.Models;

namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsUniquenessService
{
    Task IsUniqueBulkAsync(RawNews[] data);
}