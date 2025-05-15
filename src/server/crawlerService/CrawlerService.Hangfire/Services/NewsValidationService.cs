using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Models.Models;

namespace CrawlerService.Hangfire.Services;

class NewsValidationService : INewsValidationService
{
    public async Task ValidateAsync(ParsedNews[] data)
    {
        Console.WriteLine("Validating");
    }
}