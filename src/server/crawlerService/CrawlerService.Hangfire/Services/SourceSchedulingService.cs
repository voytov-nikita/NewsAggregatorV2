using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.Models.Models;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Hangfire.Services;

internal class SourceSchedulingService : ISourceSchedulingService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly ILogger<SourceSchedulingService> _logger;

    public SourceSchedulingService(
        IRecurringJobManager recurringJobManager,
        ILogger<SourceSchedulingService> logger)
    {
        _recurringJobManager = recurringJobManager;
        _logger = logger;
    }

    public void Schedule(NewsSource source)
    {
        string jobId = JobId(source.Id);

        if (!source.Enabled)
        {
            _recurringJobManager.RemoveIfExists(jobId);
            _logger.LogInformation("Removed job {JobId} (source disabled)", jobId);
            return;
        }

        _recurringJobManager.AddOrUpdate<INewsDownloaderService>(
            jobId,
            service => service.GetNewsAsync(source.Id),
            source.CronSchedule);

        _logger.LogInformation("Registered job {JobId} with schedule {Cron}", jobId, source.CronSchedule);
    }

    public void Unschedule(string sourceId)
    {
        string jobId = JobId(sourceId);
        _recurringJobManager.RemoveIfExists(jobId);
        _logger.LogInformation("Removed job {JobId}", jobId);
    }

    private static string JobId(string sourceId) => $"crawl-{sourceId}";
}
