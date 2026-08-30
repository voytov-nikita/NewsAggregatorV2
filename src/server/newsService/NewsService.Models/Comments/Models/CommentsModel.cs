namespace NewsService.Models.Comments.Models;

public class CommentsModel
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int Likes { get; set; }
    public int DisLikes { get; set; }
}
