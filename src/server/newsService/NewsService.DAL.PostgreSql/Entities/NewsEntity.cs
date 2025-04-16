namespace NewsService.DAL.PostgreSql.Entities;

public class NewsEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string OriginalLink { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime ReadDate { get; set; }
    public string? ImageLink { get; set; }
    public string Publisher { get; set; }
    public string PublisherLink { get; set; }
    
    public ICollection<CommentEntity> Comments { get; set; }
}