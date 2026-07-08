using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Hangfire.Services;

/// <summary>
/// Registers a recurring Hangfire job for every source at startup. Ongoing
/// updates go through <see cref="ISourceSchedulingService"/> called from the
/// CRUD controller — this hosted service is only responsible for the initial
/// bootstrap.
/// </summary>
public class HangfireSchedulerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISourceSchedulingService _scheduling;
    private readonly ILogger<HangfireSchedulerBackgroundService> _logger;

    public HangfireSchedulerBackgroundService(
        IServiceScopeFactory scopeFactory,
        ISourceSchedulingService scheduling,
        ILogger<HangfireSchedulerBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _scheduling = scheduling;
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
            _scheduling.Schedule(source);
        }
    }
}
