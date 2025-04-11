namespace NewsService.API.Models.Comments;

public class CommentsResponse
{
    public int Id { get; set; }
    //Todo: Investigate about Users
    /*public string CreatorName { get; set; }
    public string CreatorGuid { get; set; }*/
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int Likes { get; set; }
    public int DisLikes { get; set; }
}