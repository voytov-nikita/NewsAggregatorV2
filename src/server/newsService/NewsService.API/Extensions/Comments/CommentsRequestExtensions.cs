using NewsService.API.Models.Comments;
using NewsService.Models.Comments;

namespace NewsService.API.Extensions.Comments;

public static class CommentsRequestExtensions
{
    public static CommentsResponse ToResponse(this CommentsModel model)
    {
        return new CommentsResponse
        {
            Id = model.Id,
            CreatorName = model.CreatorName,
            CreatorGuid = model.CreatorGuid,
            Content = model.Content,
            CreateDate = model.CreateDate,
            Likes = model.Likes,
            DisLikes = model.DisLikes,
        };
    }
    
    public static CommentUpdateModel ToModel(this CommentUpdateRequest model, int newsId, int commentId)
    {
        return new CommentUpdateModel
        {
            NewsId = newsId,
            CommentId = commentId,
            CreatorGuid = model.CreatorGuid,
            Content = model.Content,
        };
    }
    
    public static CommentCreateModel ToModel(this CommentCreateRequest model, int newsId)
    {
        return new CommentCreateModel
        {
            NewsId = newsId,
            CreatorGuid = model.CreatorGuid,
            Content = model.Content,
        };
    }
    
    public static CommentsFilterModel ToModel(this CommentsFilterRequest filterRequest)
    {
        return new CommentsFilterModel
        {
            Take = filterRequest.Take,
            Offset = filterRequest.Offset,
            OrderBy = filterRequest.OrderBy,
            OrderDirection = filterRequest.OrderDirection,
        };
    }
}