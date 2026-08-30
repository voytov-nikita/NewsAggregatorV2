namespace NewsService.API.Models.Votes;

public class NewsVoteRequest
{
    /// <summary>+1 like, -1 dislike, 0 retracts the caller's vote.</summary>
    public short Value { get; set; }
}
