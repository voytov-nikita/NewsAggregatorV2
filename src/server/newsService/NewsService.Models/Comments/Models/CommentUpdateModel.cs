namespace NewsService.Models.Comments.Models;

public class CommentUpdateModel
{
    public int NewsId { get; set; }
    public int CommentId { get; set; }
    public string Content { get; set; }
}
