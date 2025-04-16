using Microsoft.EntityFrameworkCore;
using NewsService.DAL.Abstractions.Stores;
using NewsService.DAL.PostgreSql.Entities;
using NewsService.Models.News;

namespace NewsService.DAL.PostgreSql.Stores;

public class NewsStore: INewsStore
{
    private readonly NewsServiceDbContext _dbContext;

    public NewsStore(NewsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NewsModel[]> GetManyAsync(NewsFilterModel filter)
    {
        //Todo: Add filter query
        return await _dbContext.News
            .Skip(filter.Offset)
            .Take(filter.Take).Select(_ => new NewsModel
            {
                Id = _.Id,
                Title = _.Title,
                Description = _.Description,
                OriginalLink = _.OriginalLink,
                PublishDate = _.PublishDate,
                ReadDate = _.ReadDate,
                ImageLink = _.ImageLink,
                Publisher = _.Publisher,
                PublisherLink = _.PublisherLink,
            }).ToArrayAsync();
    }

    public async Task CreateAsync(NewsCreateModel model)
    {
        NewsEntity newsEntity = new NewsEntity
        {
            Title = model.Title,
            Description = model.Description,
            OriginalLink = model.OriginalLink,
            PublishDate = model.PublishDate,
            ReadDate = DateTime.Now,
            ImageLink = model.ImageLink,
            Publisher = model.Publisher,
            PublisherLink = model.PublisherLink,
        };
        
        await _dbContext.News.AddAsync(newsEntity);
    }
}