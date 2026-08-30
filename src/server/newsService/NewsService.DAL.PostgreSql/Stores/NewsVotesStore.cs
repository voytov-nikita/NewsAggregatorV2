using Microsoft.EntityFrameworkCore;
using NewsService.DAL.Abstractions.Stores;
using NewsService.DAL.PostgreSql.Entities;
using NewsService.Models.Votes.Models;

namespace NewsService.DAL.PostgreSql.Stores;

public class NewsVotesStore : INewsVotesStore
{
    private readonly NewsServiceDbContext _dbContext;

    public NewsVotesStore(NewsServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NewsVoteResultModel> VoteAsync(NewsVoteModel model)
    {
        // The context is configured with EnableRetryOnFailure, and a retrying execution strategy
        // refuses user-initiated transactions unless the whole unit of work is handed to it - it has
        // to be able to replay the transaction from the start.
        return await _dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            NewsEntity? news = await _dbContext.News
                .AsTracking()
                .FirstOrDefaultAsync(_ => _.Id == model.NewsId);

            if (news is null)
            {
                throw new KeyNotFoundException($"News {model.NewsId} not found");
            }

            NewsVoteEntity? vote = await _dbContext.Votes
                .AsTracking()
                .FirstOrDefaultAsync(_ => _.NewsId == model.NewsId && _.UserId == model.UserId);

            if (model.Value == 0)
            {
                if (vote is not null)
                {
                    _dbContext.Votes.Remove(vote);
                }
            }
            else if (vote is null)
            {
                await _dbContext.Votes.AddAsync(new NewsVoteEntity
                {
                    NewsId = model.NewsId,
                    UserId = model.UserId,
                    Value = model.Value,
                    CreateDate = DateTime.UtcNow,
                });
            }
            else if (vote.Value != model.Value)
            {
                vote.Value = model.Value;
                vote.LastModifiedDate = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            // Likes/Dislikes on News stay denormalized for the feed query, so they are recounted
            // from the Votes rows - the only source of truth - inside the same transaction.
            news.Likes = await _dbContext.Votes.CountAsync(_ => _.NewsId == model.NewsId && _.Value > 0);
            news.Dislikes = await _dbContext.Votes.CountAsync(_ => _.NewsId == model.NewsId && _.Value < 0);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new NewsVoteResultModel
            {
                Likes = news.Likes,
                Dislikes = news.Dislikes,
                MyVote = model.Value,
            };
        });
    }

    public async Task<NewsVoteResultModel> GetAsync(int newsId, Guid userId)
    {
        NewsEntity? news = await _dbContext.News
            .Where(_ => _.Id == newsId)
            .Select(_ => new NewsEntity { Likes = _.Likes, Dislikes = _.Dislikes })
            .FirstOrDefaultAsync();

        if (news is null)
        {
            throw new KeyNotFoundException($"News {newsId} not found");
        }

        short myVote = await _dbContext.Votes
            .Where(_ => _.NewsId == newsId && _.UserId == userId)
            .Select(_ => _.Value)
            .FirstOrDefaultAsync();

        return new NewsVoteResultModel
        {
            Likes = news.Likes,
            Dislikes = news.Dislikes,
            MyVote = myVote,
        };
    }
}
