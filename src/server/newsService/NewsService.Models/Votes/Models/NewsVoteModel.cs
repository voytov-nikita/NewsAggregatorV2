namespace NewsService.Models.Votes.Models;

public class NewsVoteModel
{
    public int NewsId { get; set; }

    /// <summary>Taken from ICurrentUser, never from the request body.</summary>
    public Guid UserId { get; set; }

    /// <summary>+1 like, -1 dislike, 0 retracts an existing vote.</summary>
    public short Value { get; set; }
}
