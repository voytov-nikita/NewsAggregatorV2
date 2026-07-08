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

    public async Task<OffsetCollection<NewsModel>> GetManyAsync(NewsFilterModel filter)
    {
        IQueryable<NewsEntity> filtered = ApplyFiltering(_dbContext.News, filter);

        int totalCount = await filtered.CountAsync();

        IQueryable<NewsEntity> ordered = ApplyOrdering(filtered, filter);
        var pager = new OffsetPagination(filter.Offset, filter.Take);
        IQueryable<NewsEntity> paged = ordered.ApplyPager(pager);

        NewsModel[] items = await paged.Select(_ => new NewsModel
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
            Category = _.Category,
            ReadTimeMinutes = _.ReadTimeMinutes,
            Likes = _.Likes,
            Dislikes = _.Dislikes,
            CommentsCount = _.Comments.Count(),
        }).ToArrayAsync();

        return new OffsetCollection<NewsModel>(items, filter.Offset, totalCount);
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
            Guid = model.Guid,
            Category = model.Category,
            ReadTimeMinutes = model.ReadTimeMinutes,
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
                Guid = model.Guid,
                Category = model.Category,
                ReadTimeMinutes = model.ReadTimeMinutes,
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
            NewsOrderField.Title         => query.OrderBy(q => q.Title, filter.OrderDirection),
            NewsOrderField.PublishDate   => query.OrderBy(q => q.PublishDate, filter.OrderDirection),
            NewsOrderField.MostLiked     => query.OrderBy(q => q.Likes, filter.OrderDirection),
            NewsOrderField.MostDiscussed => query.OrderBy(q => q.Comments.Count(), filter.OrderDirection),
            _                            => query.OrderBy(q => q.PublishDate, filter.OrderDirection),
        };

        return query;
    }

    private IQueryable<NewsEntity> ApplyFiltering(IQueryable<NewsEntity> query, NewsFilterModel filter)
    {
        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            string keyword = filter.Keyword;
            query = query.Where(q => q.Title.Contains(keyword) || (q.Description != null && q.Description.Contains(keyword)));
        }

        if (filter.Categories is { Length: > 0 } cats)
        {
            query = query.Where(q => cats.Contains(q.Category));
        }

        if (filter.Sources is { Length: > 0 } srcs)
        {
            query = query.Where(q => srcs.Contains(q.Publisher));
        }

        if (filter.DateFrom.HasValue)
        {
            DateTime from = filter.DateFrom.Value;
            query = query.Where(q => q.PublishDate >= from);
        }

        if (filter.DateTo.HasValue)
        {
            DateTime to = filter.DateTo.Value;
            query = query.Where(q => q.PublishDate <= to);
        }

        return query;
    }

    #endregion
}
