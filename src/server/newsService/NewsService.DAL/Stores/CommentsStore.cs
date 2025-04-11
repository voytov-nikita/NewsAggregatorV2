using Microsoft.EntityFrameworkCore;
using NewsService.DAL.Abstractions;
using NewsService.DAL.Entities;
using NewsService.Models.Comments;

namespace NewsService.DAL.Stores;

public class CommentsStore : ICommentsStore
{
    private readonly NewsServiceDbContext _dbContext;

    public CommentsStore(NewsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter)
    {
        return await _dbContext.Comments
            .Skip(filter.Offset)
            .Take(filter.Take).Select(_ => new CommentsModel
            {
                Id = _.Id,
                Content = _.Content,
                CreateDate = _.CreateDate,
                LastModifiedDate = _.LastModifiedDate,
                Likes = _.Likes,
                DisLikes = _.Dislikes,
            }).ToArrayAsync();
    }

    public async Task UpdateAsync(CommentUpdateModel model)
    {
        await _dbContext.Comments.Where(_ => _.NewsId == model.NewsId && _.Id == model.CommentId)
            .ExecuteUpdateAsync(_ =>
                _.SetProperty(_ => _.Content, model.Content)
                    .SetProperty(_ => _.LastModifiedDate, DateTime.UtcNow)
            );
    }

    public async Task CreateAsync(CommentCreateModel model)
    {
        CommentEntity commentEntity = new CommentEntity()
        {
            NewsId = model.NewsId,
            Content = model.Content,
            CreateDate = DateTime.UtcNow,
            Likes = 0,
            Dislikes = 0
        };

        await _dbContext.Comments.AddAsync(commentEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RateAsync(int newsId, int commentId, string rateType)
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

        if (rateType == "Like")
        {
            comment.Likes++;
        }
        else if (rateType == "Dislike")
        {
            comment.Dislikes++;
        }
        else
        {
            throw new ArgumentException("Invalid rateType");
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int newsId, int commentId)
    {
        await _dbContext.Comments.Where(_ => _.NewsId == newsId && _.Id == commentId).ExecuteDeleteAsync();
    }
}