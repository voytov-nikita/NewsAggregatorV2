namespace CrawlerService.Models.Models;

/// <summary>
/// CSS selectors used by the HTML crawler to extract news items from a page.
/// All selectors except <see cref="ItemSelector"/>, <see cref="Title"/> and <see cref="Link"/>
/// are optional.
/// </summary>
public class HtmlSelectors
{
    /// <summary>CSS selector matching each news item container on the page.</summary>
    public string ItemSelector { get; set; } = null!;

    public FieldMapping Title { get; set; } = null!;
    public FieldMapping Link { get; set; } = null!;
    public FieldMapping? Description { get; set; }
    public FieldMapping? Image { get; set; }
    public FieldMapping? PublishDate { get; set; }
    public FieldMapping? Guid { get; set; }
    public FieldMapping? Tags { get; set; }
}
