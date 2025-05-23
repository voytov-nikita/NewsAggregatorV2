using NewsService.BLL.Abstractions;
using NewsService.BLL.Abstractions.Services;
using NewsService.BLL.Abstractions.Validators;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Services;

public class NewsService: INewsService
{
    private readonly INewsStore _store;
    private readonly INewsValidator _validator;

    public NewsService(INewsStore store, INewsValidator validator)
    {
        _store = store;
        _validator = validator;
    }

    public async Task<NewsModel[]> GetManyAsync(NewsFilterModel filter)
    {
        return await _store.GetManyAsync(filter);
    }
    public async Task CreateAsync(NewsCreateModel model)
    {
        _validator.Validate(model);
        
        await _store.CreateAsync(model);
    }
    public async Task CreateBulkAsync(NewsCreateModel[] models)
    {
        _validator.ValidateBulk(models);
        
        await _store.CreateBulkAsync(models);
    }
}