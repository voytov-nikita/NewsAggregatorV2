namespace NewsService.API.Models.Comments;

public class CommentUpdateRequest
{
    public string CreatorGuid { get; set; }
    public string Content { get; set; }
}