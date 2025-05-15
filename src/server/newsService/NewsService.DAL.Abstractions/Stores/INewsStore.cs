using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.DAL.Abstractions.Stores;

public interface INewsStore
{
    public Task<NewsModel[]> GetManyAsync(NewsFilterModel filter);
    public Task CreateAsync(NewsCreateModel model);
    public Task CreateBulkAsync(NewsCreateModel[] createModels);
}