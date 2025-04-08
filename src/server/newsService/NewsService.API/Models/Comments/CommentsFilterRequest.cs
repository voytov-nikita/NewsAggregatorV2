namespace NewsService.API.Models.Comments;

public class CommentsFilterRequest
{
    public int Take { get; set; }
    public int Offset { get; set; }
    public string OrderBy { get; set; }
    public string OrderDirection { get; set; }
}