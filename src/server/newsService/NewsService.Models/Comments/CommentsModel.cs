namespace NewsService.Models.Comments;

public class CommentsModel
{
    public int Id { get; set; }
    public string CreatorName { get; set; }
    public string CreatorGuid { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public int Likes { get; set; }
    public int DisLikes { get; set; }
}