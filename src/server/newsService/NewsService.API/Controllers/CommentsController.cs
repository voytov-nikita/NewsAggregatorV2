using Microsoft.AspNetCore.Mvc;
using NewsService.API.Extensions.Comments;
using NewsService.API.Models.Comments;
using NewsService.BLL.Abstractions;
using NewsService.Models.Comments;

namespace NewsService.API.Controllers;


[ApiController]
[Route("api/v1/news/{newsId:int}/comments")]
public class CommentsController: ControllerBase
{
    private readonly ICommentsService _commentsService;

    public CommentsController(ICommentsService commentsService)
    {
        _commentsService = commentsService;
    }

    [HttpGet]
    public async Task<List<CommentsResponse>> GetMany(CommentsFilterRequest filterRequest)
    {
        CommentsFilterModel filter = filterRequest.ToModel();

        CommentsModel[] result = await _commentsService.GetManyAsync(filter);
        
        //AddHeaders
        
        return result.Select(_ => _.ToResponse()).ToList();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] int newsId, CommentCreateRequest request)
    {
        CommentCreateModel model = request.ToModel();
        await _commentsService.CreateAsync(model);
        
        return Ok();
    }
    
    [HttpPut("/{commentId:int}")]
    public async Task<IActionResult> Update([FromRoute] int newsId, [FromRoute] int commentId, CommentUpdateRequest request)
    {
        CommentUpdateModel model = request.ToModel();
        
        await _commentsService.UpdateAsync(model);

        return Ok();
    }
    
    [HttpPut("/{commentId:int}/like")]
    public async Task<IActionResult> Like([FromRoute] int newsId, [FromRoute] int commentId)
    {
        await _commentsService.RateAsync(newsId, commentId, "Like");

        return Ok();
    }
    
    [HttpPut("/{commentId:int}/dislike")]
    public async Task<IActionResult> Dislike([FromRoute] int newsId, [FromRoute] int commentId)
    {
        await _commentsService.RateAsync(newsId, commentId, "Dislike");

        return Ok();
    }
    
}
