using Hangfire;
using Microsoft.Extensions.Hosting;

namespace CrawlerService.Hangfire.Services;

public class HangfireSchedulerBackgroundService: BackgroundService
{
    private readonly IRecurringJobManager _recurringJobManager;

    public HangfireSchedulerBackgroundService(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Todo: add opportunity of changing schedule by API request
        
        _recurringJobManager.AddOrUpdate<NewsDownloaderService>("crawl-news", service => service.GetNewsAsync(), "*/30 * * * * *"); // Every 30 seconds. 
        
        return Task.CompletedTask;
    }
}