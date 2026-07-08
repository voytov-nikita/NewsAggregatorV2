using Common.Models;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Abstractions.Services;

public interface INewsService
{
    public Task<OffsetCollection<NewsModel>> GetManyAsync(NewsFilterModel filter);
    public Task CreateAsync(NewsCreateModel model);
    public Task CreateBulkAsync(NewsCreateModel[] models);
}
