using NewsService.API.Models.Comments;
using NewsService.Models.Comments;
using NewsService.Models.Comments.Models;

namespace NewsService.API.Extensions.Comments;

public static class CommentsRequestExtensions
{
    public static CommentsResponse ToResponse(this CommentsModel model)
    {
        return new CommentsResponse
        {
            Id = model.Id,
            AuthorId = model.AuthorId,
            AuthorName = model.AuthorName,
            Content = model.Content,
            CreateDate = model.CreateDate,
            LastModifiedDate = model.LastModifiedDate,
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
            Content = model.Content,
        };
    }

    /// <summary>
    /// The author is passed in from ICurrentUser: the request body has no say in who wrote a comment.
    /// </summary>
    public static CommentCreateModel ToModel(this CommentCreateRequest model, int newsId, Guid authorId, string authorName)
    {
        return new CommentCreateModel
        {
            NewsId = newsId,
            AuthorId = authorId,
            AuthorName = authorName,
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
