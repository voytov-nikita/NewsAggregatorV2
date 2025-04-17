using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Abstractions;

public interface INewsService
{
    public Task<NewsModel[]> GetManyAsync(NewsFilterModel filter);
    public Task CreateAsync(NewsCreateModel model);
}