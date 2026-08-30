using NewsService.Models.Votes.Models;

namespace NewsService.BLL.Abstractions.Services;

public interface INewsVotesService
{
    public Task<NewsVoteResultModel> VoteAsync(NewsVoteModel model);
    public Task<NewsVoteResultModel> GetAsync(int newsId, Guid userId);
}
