using Common.Models;

namespace NewsService.Models.News.Models;


public class NewsModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string OriginalLink { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime ReadDate { get; set; }
    public string? ImageLink { get; set; }
    public string Publisher { get; set; }
    public string PublisherLink { get; set; }
    public string Guid { get; set; }

    public NewsCategory Category { get; set; }
    public int ReadTimeMinutes { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int CommentsCount { get; set; }
}