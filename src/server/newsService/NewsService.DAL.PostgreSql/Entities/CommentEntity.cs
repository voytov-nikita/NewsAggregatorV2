namespace NewsService.DAL.PostgreSql.Entities;

public class CommentEntity
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    //Todo: Investigate about Users
    /*public string CreatorName { get; set; }
    public string CreatorGuid { get; set; }*/
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    
    public NewsEntity News { get; set; }
}