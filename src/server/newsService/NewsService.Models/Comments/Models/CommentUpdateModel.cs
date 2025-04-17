namespace NewsService.Models.Comments.Models;

public class CommentUpdateModel
{
    public int NewsId { get; set; }
    public int CommentId { get; set; }
    //Todo: Investigate about Users
    /*public string CreatorName { get; set; }
    public string CreatorGuid { get; set; }*/
    public string Content { get; set; }
}