using NewsService.API.Models.Comments;
using NewsService.Models.Comments;

namespace NewsService.API.Extensions.Comments;

public static class CommentsRequestExtensions
{
    public static CommentsResponse ToResponse(this CommentsModel model)
    {
        return new CommentsResponse() { };
    }
    
    public static CommentUpdateModel ToModel(this CommentUpdateRequest model)
    {
        return new CommentUpdateModel() { };
    }
    
    public static CommentCreateModel ToModel(this CommentCreateRequest model)
    {
        return new CommentCreateModel() { };
    }
    
    public static CommentsFilterModel ToModel(this CommentsFilterRequest filterRequest)
    {
        return new CommentsFilterModel() { };
    }
}