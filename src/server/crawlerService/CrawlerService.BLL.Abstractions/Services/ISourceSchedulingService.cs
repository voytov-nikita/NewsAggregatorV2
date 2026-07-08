using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Abstractions.Services;

/// <summary>
/// Owns the mapping between a <see cref="NewsSource"/> and the Hangfire recurring
/// job that crawls it. Both the startup scheduler and the CRUD controller go
/// through this one place so the job graph in Hangfire always mirrors the
/// current Mongo collection.
/// </summary>
public interface ISourceSchedulingService
{
    /// <summary>Register or update a recurring job for a source. No-op when disabled — the job is removed instead.</summary>
    void Schedule(NewsSource source);

    /// <summary>Remove the recurring job for a source id, if any.</summary>
    void Unschedule(string sourceId);
}
