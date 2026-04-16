using AngleSharp.Dom;
using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Services.Mapping;

/// <summary>
/// Applies a <see cref="FieldMapping"/> to an HTML container element.
/// The <see cref="FieldMapping.Selector"/> is interpreted as a CSS selector
/// relative to the container.
/// </summary>
internal static class HtmlFieldExtractor
{
    public static string? ExtractSingle(IElement container, FieldMapping? mapping)
    {
        if (mapping is null || string.IsNullOrWhiteSpace(mapping.Selector))
        {
            return null;
        }

        IElement? element = container.QuerySelector(mapping.Selector);
        if (element is null)
        {
            return null;
        }

        return ReadValue(element, mapping);
    }

    public static string[] ExtractMany(IElement container, FieldMapping? mapping)
    {
        if (mapping is null || string.IsNullOrWhiteSpace(mapping.Selector))
        {
            return Array.Empty<string>();
        }

        IHtmlCollection<IElement> elements = container.QuerySelectorAll(mapping.Selector);
        if (elements.Length == 0)
        {
            return Array.Empty<string>();
        }

        List<string> values = new(elements.Length);
        foreach (IElement element in elements)
        {
            string? value = ReadValue(element, mapping);
            if (!string.IsNullOrWhiteSpace(value))
            {
                values.Add(value);
            }
        }

        return values.ToArray();
    }

    private static string? ReadValue(IElement element, FieldMapping mapping)
    {
        if (mapping.From == FieldValueSource.Attribute)
        {
            if (string.IsNullOrEmpty(mapping.Attribute))
            {
                return null;
            }
            return element.GetAttribute(mapping.Attribute);
        }

        return element.TextContent?.Trim();
    }
}
