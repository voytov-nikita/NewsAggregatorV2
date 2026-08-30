using Common.Auth.Authorization;
using Common.Auth.Constants;
using Common.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsService.API.Extensions.Comments;
using NewsService.API.Models.Comments;
using NewsService.BLL.Abstractions;
using NewsService.BLL.Abstractions.Services;
using NewsService.Models.Comments;
using NewsService.Models.Comments.Enums;
using NewsService.Models.Comments.Models;

namespace NewsService.API.Controllers;


[ApiController]
[Route("api/v1/news/{newsId:int}/comments")]
public class CommentsController: ControllerBase
{
    private readonly ICommentsService _commentsService;
    private readonly ICurrentUser _currentUser;

    public CommentsController(ICommentsService commentsService, ICurrentUser currentUser)
    {
        _commentsService = commentsService;
        _currentUser = currentUser;
    }

    /// <summary>Anonymous by design - reading the site never requires an account.</summary>
    [HttpGet("")]
    public async Task<List<CommentsResponse>> GetMany([FromQuery] CommentsFilterRequest filterRequest, int newsId)
    {
        CommentsFilterModel filter = filterRequest.ToModel();

        CommentsModel[] result = await _commentsService.GetManyAsync(filter, newsId);
        
        //AddHeaders
        
        return result.Select(_ => _.ToResponse()).ToList();
    }
    
    [HttpPost("")]
    [HasPermission(Permissions.CommentWrite)]
    public async Task<IActionResult> Create([FromRoute] int newsId, CommentCreateRequest request)
    {
        CommentCreateModel model = request.ToModel(newsId, _currentUser.UserId!.Value, AuthorName());
        
        await _commentsService.CreateAsync(model);
        
        return Ok();
    }

    /// <summary>
    /// Ownership is enforced in the BLL, not here: it needs the stored AuthorId, which makes it a
    /// business rule rather than a routing concern.
    /// </summary>
    [HttpPut("{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] int newsId, [FromRoute] int commentId, CommentUpdateRequest request)
    {
        CommentUpdateModel model = request.ToModel(newsId, commentId);
        
        await _commentsService.UpdateAsync(model);

        return Ok();
    }

    [HttpDelete("{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int newsId, [FromRoute] int commentId)
    {
        await _commentsService.DeleteAsync(newsId, commentId);

        return NoContent();
    }
    
    [HttpPut("{commentId:int}/like")]
    [Authorize]
    public async Task<IActionResult> Like([FromRoute] int newsId, [FromRoute] int commentId)
    {
        await _commentsService.RateAsync(newsId, commentId, RateType.Like);

        return Ok();
    }
    
    [HttpPut("{commentId:int}/dislike")]
    [Authorize]
    public async Task<IActionResult> Dislike([FromRoute] int newsId, [FromRoute] int commentId)
    {
        await _commentsService.RateAsync(newsId, commentId, RateType.Dislike);

        return Ok();
    }

    #region private

    /// <summary>
    /// Snapshot of the author's name at write time. Falls back to the email, then to a placeholder,
    /// so a token missing the optional `name` claim still produces a readable comment.
    /// </summary>
    private string AuthorName() => _currentUser.DisplayName ?? _currentUser.Email ?? "Unknown user";

    #endregion
}
