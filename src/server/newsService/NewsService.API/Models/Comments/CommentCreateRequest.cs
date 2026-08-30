namespace NewsService.API.Models.Comments;

public class CommentCreateRequest
{
    // No author field on purpose: the author comes from the caller's token, never from the body.
    public string Content { get; set; }
}
