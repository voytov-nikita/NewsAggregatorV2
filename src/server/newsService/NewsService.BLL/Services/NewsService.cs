using NewsService.BLL.Abstractions;
using NewsService.DAL.Abstractions;
using NewsService.Models.News;

namespace NewsService.BLL.Services;

public class NewsService: INewsService
{
    private readonly INewsStore _store;

    public NewsService(INewsStore store)
    {
        _store = store;
    }

    public async Task<NewsModel[]> GetManyAsync(NewsFilterModel filter)
    {
        return await _store.GetManyAsync(filter);
    }
    public async Task CreateAsync(NewsCreateModel model)
    {
        await _store.CreateAsync(model);
    }
}