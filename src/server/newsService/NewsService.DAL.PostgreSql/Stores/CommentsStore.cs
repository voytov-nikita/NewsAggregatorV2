using Common.Extensions;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using NewsService.DAL.Abstractions.Stores;
using NewsService.DAL.PostgreSql.Entities;
using NewsService.DAL.PostgreSql.Extensions;
using NewsService.Models.Comments.Enums;
using NewsService.Models.Comments.Models;

namespace NewsService.DAL.PostgreSql.Stores;

public class CommentsStore : ICommentsStore
{
    private readonly NewsServiceDbContext _dbContext;

    public CommentsStore(NewsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter, int newsId)
    {
        IQueryable<CommentEntity> query = _dbContext.Comments.Where(q => q.NewsId == newsId);
        query = ApplyOrdering(query, filter);
        
        var pager = new OffsetPagination(filter.Offset, filter.Take);
        
        query = query.ApplyPager(pager);
        
        return await query.Select(_ => new CommentsModel
            {
                Id = _.Id,
                NewsId = _.NewsId,
                AuthorId = _.AuthorId,
                AuthorName = _.AuthorName,
                Content = _.Content,
                CreateDate = _.CreateDate,
                LastModifiedDate = _.LastModifiedDate,
                Likes = _.Likes,
                DisLikes = _.Dislikes,
            }).ToArrayAsync();
    }

    public async Task UpdateAsync(CommentUpdateModel model)
    {
        //Todo: Add error handling when NewsId and CommentId not exist
        await _dbContext.Comments.Where(_ => _.NewsId == model.NewsId && _.Id == model.CommentId)
            .ExecuteUpdateAsync(_ =>
                _.SetProperty(_ => _.Content, model.Content)
                    .SetProperty(_ => _.LastModifiedDate, DateTime.UtcNow)
            );
    }

    public async Task CreateAsync(CommentCreateModel model)
    {
        //Todo: Add error handling when NewsId not exist
        CommentEntity commentEntity = new CommentEntity()
        {
            NewsId = model.NewsId,
            AuthorId = model.AuthorId,
            AuthorName = model.AuthorName,
            Content = model.Content,
            CreateDate = DateTime.UtcNow,
            Likes = 0,
            Dislikes = 0
        };

        await _dbContext.Comments.AddAsync(commentEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RateAsync(int newsId, int commentId, RateType rateType)
    {
        //Todo: Rate system will be reworked in future
        CommentEntity? comment = await _dbContext.Comments
            .AsTracking()
            .FirstOrDefaultAsync(
                _ => _.NewsId == newsId && _.Id == commentId
            );

        if (comment == null)
        {
            throw new ApplicationException("Comment not found");
        }

        switch (rateType)
        {
            case RateType.Like:
                comment.Likes++;
                break;
            case RateType.Dislike:
                comment.Dislikes++;
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof(rateType), $"Unexpected rateType value '{rateType}'");
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Guid?> GetAuthorIdAsync(int newsId, int commentId)
    {
        return await _dbContext.Comments
            .Where(_ => _.NewsId == newsId && _.Id == commentId)
            .Select(_ => (Guid?)_.AuthorId)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(int newsId, int commentId)
    {
        await _dbContext.Comments.Where(_ => _.NewsId == newsId && _.Id == commentId).ExecuteDeleteAsync();
    }

    #region private

    
    private IQueryable<CommentEntity> ApplyOrdering(IQueryable<CommentEntity> query, CommentsFilterModel filter)
    {
        query = query.OrderBy(q => q.CreateDate, filter.OrderDirection);
        
        return query;
    }


    #endregion
}