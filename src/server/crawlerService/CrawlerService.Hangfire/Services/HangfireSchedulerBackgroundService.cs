using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Hangfire.Services;

public class HangfireSchedulerBackgroundService : BackgroundService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HangfireSchedulerBackgroundService> _logger;

    public HangfireSchedulerBackgroundService(
        IRecurringJobManager recurringJobManager,
        IServiceScopeFactory scopeFactory,
        ILogger<HangfireSchedulerBackgroundService> logger)
    {
        _recurringJobManager = recurringJobManager;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISourceStore sourceStore = scope.ServiceProvider.GetRequiredService<ISourceStore>();

        NewsSource[] sources = await sourceStore.GetAllEnabledAsync();
        _logger.LogInformation("Registering recurring jobs for {Count} enabled sources", sources.Length);

        foreach (NewsSource source in sources)
        {
            string jobId = $"crawl-{source.Id}";
            _recurringJobManager.AddOrUpdate<INewsDownloaderService>(
                jobId,
                service => service.GetNewsAsync(source.Id),
                source.CronSchedule);

            _logger.LogInformation("Registered job {JobId} with schedule {Cron}", jobId, source.CronSchedule);
        }
    }
}
