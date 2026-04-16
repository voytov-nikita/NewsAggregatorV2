using CrawlerService.Models.Models;

namespace CrawlerService.DAL.Abstractions.Stores;

public interface ISourceStore
{
    Task<NewsSource[]> GetAllEnabledAsync();
    Task<NewsSource?> GetByIdAsync(string id);
    Task UpsertAsync(NewsSource source);
}
