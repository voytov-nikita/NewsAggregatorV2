using NewsService.API.Models.Votes;
using NewsService.Models.Votes.Models;

namespace NewsService.API.Extensions.Votes;

public static class VotesRequestExtensions
{
    /// <summary>The user id is a parameter, not a request field - a client-supplied voter is a fraud.</summary>
    public static NewsVoteModel ToModel(this NewsVoteRequest request, int newsId, Guid userId)
    {
        return new NewsVoteModel
        {
            NewsId = newsId,
            UserId = userId,
            Value = request.Value,
        };
    }

    public static NewsVoteResponse ToResponse(this NewsVoteResultModel model)
    {
        return new NewsVoteResponse
        {
            Likes = model.Likes,
            Dislikes = model.Dislikes,
            MyVote = model.MyVote,
        };
    }
}
