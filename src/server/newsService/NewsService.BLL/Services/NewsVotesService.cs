using NewsService.BLL.Abstractions.Services;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.Votes.Models;

namespace NewsService.BLL.Services;

public class NewsVotesService : INewsVotesService
{
    private readonly INewsVotesStore _newsVotesStore;

    public NewsVotesService(INewsVotesStore newsVotesStore)
    {
        _newsVotesStore = newsVotesStore;
    }

    public async Task<NewsVoteResultModel> VoteAsync(NewsVoteModel model)
    {
        if (model.Value is not (-1 or 0 or 1))
        {
            throw new ArgumentException("Vote value must be 1, -1 or 0.", nameof(model));
        }

        return await _newsVotesStore.VoteAsync(model);
    }

    public async Task<NewsVoteResultModel> GetAsync(int newsId, Guid userId)
    {
        return await _newsVotesStore.GetAsync(newsId, userId);
    }
}
