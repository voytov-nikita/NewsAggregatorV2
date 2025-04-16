using NewsService.Models.News;

namespace NewsService.BLL.Abstractions;

public interface INewsService
{
    public Task<NewsModel[]> GetManyAsync(NewsFilterModel filter);
    public Task CreateAsync(NewsCreateModel model);
}