namespace CrawlerService.Models.Models;

/// <summary>
/// Optional override for non-standard feed sources. When null, the feed crawler uses
/// default <c>SyndicationFeed</c> parsing (covers standard RSS 2.0 and Atom 1.0).
/// When provided, all fields are resolved via XPath selectors over the raw XML.
/// </summary>
public class FeedFieldMapping
{
    /// <summary>XPath selecting each item node. Default: <c>//item</c>.</summary>
    public string? ItemXPath { get; set; }

    public FieldMapping? Title { get; set; }
    public FieldMapping? Description { get; set; }
    public FieldMapping? Link { get; set; }
    public FieldMapping? Image { get; set; }
    public FieldMapping? PublishDate { get; set; }
    public FieldMapping? Guid { get; set; }
}
