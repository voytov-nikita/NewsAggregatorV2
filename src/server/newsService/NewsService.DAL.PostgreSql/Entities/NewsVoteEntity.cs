namespace NewsService.DAL.PostgreSql.Entities;

/// <summary>
/// One row per (news, user). The unique index is what makes voting idempotent - without it the
/// Likes/Dislikes counters could be incremented without bound by the same user.
/// </summary>
public class NewsVoteEntity
{
    public int Id { get; set; }
    public int NewsId { get; set; }
    public Guid UserId { get; set; }

    /// <summary>+1 for a like, -1 for a dislike. A retracted vote deletes the row instead of storing 0.</summary>
    public short Value { get; set; }

    public DateTime CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    public NewsEntity News { get; set; }
}
