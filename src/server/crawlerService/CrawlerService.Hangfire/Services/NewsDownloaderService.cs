using CrawlerService.Hangfire.Abstractions.Services;
using Hangfire;

namespace CrawlerService.Hangfire.Services;

public class NewsDownloaderService: INewsDownloaderService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NewsDownloaderService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }
    
    public Task GetNewsAsync()
    {
        Console.WriteLine("Crawling...");
        Console.WriteLine("Crawling complete");

        byte[] data = [];

        _backgroundJobClient.Enqueue<INewsParserService>(service => service.ParseAsync(data));
        
        return Task.CompletedTask;
    }
    
}