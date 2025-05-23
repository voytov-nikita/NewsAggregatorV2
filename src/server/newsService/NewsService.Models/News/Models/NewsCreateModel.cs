namespace NewsService.Models.News.Models;


public class NewsCreateModel
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string OriginalLink { get; set; }
    public string? ImageLink { get; set; }
    public DateTime PublishDate { get; set; }
    public string Publisher { get; set; }
    public string PublisherLink { get; set; }
    public string Guid { get; set; }
}