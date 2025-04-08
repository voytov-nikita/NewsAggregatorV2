namespace NewsService.Models.Comments;

public class CommentCreateModel
{
    public int NewsId { get; set; }
    public string CreatorGuid { get; set; }
    public string Content { get; set; }
}