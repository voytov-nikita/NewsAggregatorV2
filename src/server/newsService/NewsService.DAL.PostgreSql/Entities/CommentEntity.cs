namespace NewsService.DAL.PostgreSql.Entities;

public class CommentEntity
{
    public int Id { get; set; }
    public int NewsId { get; set; }

    public Guid AuthorId { get; set; }

    /// <summary>
    /// Denormalized snapshot of the author's display name, taken from the token's `name` claim when
    /// the comment is written. NewsService must not call AuthorService synchronously to render a
    /// comment list, and a foreign key cannot cross a service boundary. Consequence: renaming a user
    /// does not rename their old comments - a `user-updated` event over RabbitMQ would fix that.
    /// </summary>
    public string AuthorName { get; set; }

    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    
    public NewsEntity News { get; set; }
}
