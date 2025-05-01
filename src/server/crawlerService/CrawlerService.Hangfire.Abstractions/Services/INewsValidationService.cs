using CrawlerService.Hangfire.Models;

namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsValidationService
{
    Task ValidateAsync(RawNews[] data);
}