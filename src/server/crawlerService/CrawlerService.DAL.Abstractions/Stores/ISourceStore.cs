using CrawlerService.Models.Models;

namespace CrawlerService.DAL.Abstractions.Stores;

public interface ISourceStore
{
    Task<NewsSource[]> GetAllAsync();
    Task<NewsSource[]> GetAllEnabledAsync();
    Task<NewsSource?> GetByIdAsync(string id);
    Task UpsertAsync(NewsSource source);
    Task<bool> DeleteAsync(string id);

    Task UpdateMetricsAsync(
        string id,
        DateTime lastAt,
        bool success,
        int durationMs,
        int parsedCount);

    Task IncrementArticlesAsync(string id, int delta);
}
