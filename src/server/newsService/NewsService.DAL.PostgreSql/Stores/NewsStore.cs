using Common.Extensions;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using NewsService.DAL.Abstractions.Stores;
using NewsService.DAL.PostgreSql.Entities;
using NewsService.DAL.PostgreSql.Extensions;
using NewsService.Models.News.Enum;
using NewsService.Models.News.Models;

namespace NewsService.DAL.PostgreSql.Stores;

public class NewsStore : INewsStore
{
    private readonly NewsServiceDbContext _dbContext;

    public NewsStore(NewsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NewsModel[]> GetManyAsync(NewsFilterModel filter)
    {
        IQueryable<NewsEntity> query = _dbContext.News;

        query = ApplyOrdering(query, filter);
        query = ApplyFiltering(query, filter);

        var pager = new OffsetPagination(filter.Offset, filter.Take);
        query = query.ApplyPager(pager);

        return await query.Select(_ => new NewsModel
        {
            Id = _.Id,
            Title = _.Title,
            Description = _.Description,
            OriginalLink = _.OriginalLink,
            PublishDate = _.PublishDate,
            ReadDate = _.CreateDate,
            ImageLink = _.ImageLink,
            Publisher = _.Publisher,
            PublisherLink = _.PublisherLink,
            Guid = _.Guid,
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
            CreateDate = DateTime.UtcNow,
            ImageLink = model.ImageLink,
            Publisher = model.Publisher,
            PublisherLink = model.PublisherLink,
            Guid = model.Guid
        };

        await _dbContext.News.AddAsync(newsEntity);

        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateBulkAsync(NewsCreateModel[] createModels)
    {
        //Todo: Add handling for duplicates
        NewsEntity[] newsEntities = createModels.Select(model => new NewsEntity
            {
                Title = model.Title,
                Description = model.Description,
                OriginalLink = model.OriginalLink,
                PublishDate = model.PublishDate,
                CreateDate = DateTime.UtcNow,
                ImageLink = model.ImageLink,
                Publisher = model.Publisher,
                PublisherLink = model.PublisherLink,
                Guid = model.Guid
            }
        ).ToArray();

        await _dbContext.News.AddRangeAsync(newsEntities);

        await _dbContext.SaveChangesAsync();
    }

    #region private

    private IQueryable<NewsEntity> ApplyOrdering(IQueryable<NewsEntity> query, NewsFilterModel filter)
    {
        query = filter.OrderBy switch
        {
            NewsOrderField.Title => query.OrderBy(q => q.Title, filter.OrderDirection),
            NewsOrderField.PublishDate => query.OrderBy(q => q.PublishDate, filter.OrderDirection),
            _ => query.OrderBy(q => q.PublishDate, filter.OrderDirection),
        };

        return query;
    }

    private IQueryable<NewsEntity> ApplyFiltering(IQueryable<NewsEntity> query, NewsFilterModel filter)
    {
        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            query = query.Where(q => q.Title.Contains(filter.Keyword));
        }

        return query;
    }

    #endregion
}