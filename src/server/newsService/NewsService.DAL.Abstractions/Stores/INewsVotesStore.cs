using NewsService.Models.Votes.Models;

namespace NewsService.DAL.Abstractions.Stores;

public interface INewsVotesStore
{
    /// <summary>
    /// Casts, changes or (for value 0) retracts the user's vote and returns the recounted totals.
    /// </summary>
    public Task<NewsVoteResultModel> VoteAsync(NewsVoteModel model);

    public Task<NewsVoteResultModel> GetAsync(int newsId, Guid userId);
}
