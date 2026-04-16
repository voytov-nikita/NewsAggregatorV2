namespace CrawlerService.Models.Models;

public class ParsedNews
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string OriginalLink { get; set; }
    public string? ImageLink { get; set; }
    public DateTime PublishDate { get; set; }
    public string SourceId { get; set; }
    public string PublisherName { get; set; }
    public string PublisherLink { get; set; }
    public string PublisherGuid { get; set; }
    public string GlobalUniqueId { get; set; }
    public string Guid { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}
