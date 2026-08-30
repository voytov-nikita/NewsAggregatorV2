using Common.Auth.Authorization;
using Common.Auth.Constants;
using Common.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using NewsService.API.Extensions.Votes;
using NewsService.API.Models.Votes;
using NewsService.BLL.Abstractions.Services;
using NewsService.Models.Votes.Models;

namespace NewsService.API.Controllers;

[ApiController]
[Route("api/v1/news/{newsId:int}")]
public class NewsVotesController : ControllerBase
{
    private readonly INewsVotesService _newsVotesService;
    private readonly ICurrentUser _currentUser;

    public NewsVotesController(INewsVotesService newsVotesService, ICurrentUser currentUser)
    {
        _newsVotesService = newsVotesService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Casts, changes or retracts (value 0) the caller's vote. Idempotent: a unique index on
    /// (NewsId, UserId) means voting the same way twice leaves the counters untouched.
    /// </summary>
    [HttpPost("vote")]
    [HasPermission(Permissions.NewsVote)]
    public async Task<ActionResult<NewsVoteResponse>> Vote([FromRoute] int newsId, NewsVoteRequest request)
    {
        NewsVoteModel model = request.ToModel(newsId, _currentUser.UserId!.Value);

        NewsVoteResultModel result = await _newsVotesService.VoteAsync(model);

        return result.ToResponse();
    }

    /// <summary>Anonymous readers get the totals with MyVote = 0.</summary>
    [HttpGet("vote")]
    public async Task<ActionResult<NewsVoteResponse>> Get([FromRoute] int newsId)
    {
        NewsVoteResultModel result =
            await _newsVotesService.GetAsync(newsId, _currentUser.UserId ?? Guid.Empty);

        return result.ToResponse();
    }
}
