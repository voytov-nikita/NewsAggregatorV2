namespace NewsService.Models.Votes.Models;

public class NewsVoteResultModel
{
    public int Likes { get; set; }
    public int Dislikes { get; set; }

    /// <summary>The caller's own vote after the operation: +1, -1 or 0 when they hold none.</summary>
    public short MyVote { get; set; }
}
