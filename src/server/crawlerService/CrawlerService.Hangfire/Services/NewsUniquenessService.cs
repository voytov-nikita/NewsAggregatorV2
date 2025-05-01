using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Hangfire.Models;

namespace CrawlerService.Hangfire.Services;

class NewsUniquenessService : INewsUniquenessService
{
    public Task IsUniqueBulkAsync(RawNews[] data)
    {
        //Check on unique
        
        //Send message with news to add
        
        return Task.CompletedTask;
    }
}