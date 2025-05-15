using CrawlerService.Models.Models;

namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsValidationService
{
    Task ValidateAsync(ParsedNews[] data);
}