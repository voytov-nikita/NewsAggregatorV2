using Common.Models;

namespace MessageQueue.Models;

public class NewsQueueModel
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string OriginalLink { get; set; }
    public string? ImageLink { get; set; }
    public DateTime PublishDate { get; set; }
    public string PublisherName { get; set; }
    public string PublisherLink { get; set; }
    public string PublisherGuid { get; set; }
    public string Guid { get; set; }

    public NewsCategory Category { get; set; } = NewsCategory.Uncategorized;
}
