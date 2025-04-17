namespace NewsService.Models.Comments.Models;

public class CommentCreateModel
{
    public int NewsId { get; set; }
    //Todo: Investigate about Users
    /*public string CreatorGuid { get; set; }*/
    public string Content { get; set; }
}