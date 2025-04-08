namespace NewsService.Models.Comments;

public class CommentUpdateModel
{
    public int NewsId { get; set; }
    public int CommentId { get; set; }
    public string CreatorGuid { get; set; }
    public string Content { get; set; }
}