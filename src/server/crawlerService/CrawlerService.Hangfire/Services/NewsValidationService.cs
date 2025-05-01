using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Hangfire.Models;

namespace CrawlerService.Hangfire.Services;

class NewsValidationService : INewsValidationService
{
    public Task ValidateAsync(RawNews[] data)
    {
        throw new NotImplementedException();
    }
}