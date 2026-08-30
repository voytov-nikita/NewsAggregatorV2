namespace NewsService.Models.Comments.Models;

public class CommentCreateModel
{
    public int NewsId { get; set; }

    /// <summary>
    /// Filled from ICurrentUser by the controller, never from the request body - an author id sent
    /// by the client would let anyone post as anyone.
    /// </summary>
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; }

    public string Content { get; set; }
}
