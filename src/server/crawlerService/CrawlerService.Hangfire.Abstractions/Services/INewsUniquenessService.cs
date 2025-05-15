using CrawlerService.Models.Models;

namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsUniquenessService
{
    Task IsUniqueBulkAsync(ParsedNews[] data);
}