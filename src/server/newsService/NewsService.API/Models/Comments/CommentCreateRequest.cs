namespace NewsService.API.Models.Comments;

public class CommentCreateRequest
{
    public string CreatorGuid { get; set; }
    public string Content { get; set; }
}