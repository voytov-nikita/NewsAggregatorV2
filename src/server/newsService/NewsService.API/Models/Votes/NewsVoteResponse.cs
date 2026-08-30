namespace NewsService.API.Models.Votes;

public class NewsVoteResponse
{
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public short MyVote { get; set; }
}
