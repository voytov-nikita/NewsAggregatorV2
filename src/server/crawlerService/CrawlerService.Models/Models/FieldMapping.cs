namespace CrawlerService.Models.Models;

public enum FieldValueSource
{
    Text = 1,
    Attribute = 2,
}

public class FieldMapping
{
    public string Selector { get; set; } = null!;
    public FieldValueSource From { get; set; } = FieldValueSource.Text;
    public string? Attribute { get; set; }
}
