using NewsService.Models.News;

namespace NewsService.DAL.Abstractions;

public interface INewsStore
{
    public Task<NewsModel[]> GetManyAsync(NewsFilterModel filter);
    public Task CreateAsync(NewsCreateModel model);
}