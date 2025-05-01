using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Hangfire.Models;
using Hangfire;

namespace CrawlerService.Hangfire.Services;

class NewsParserService : INewsParserService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NewsParserService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }
    
    public Task ParseAsync(byte[] data)
    {
        //parse data
        
        RawNews[] items = [];
        
        _backgroundJobClient.Enqueue<INewsUniquenessService>(service => service.IsUniqueBulkAsync(items));
        
        return Task.CompletedTask;
    }
}