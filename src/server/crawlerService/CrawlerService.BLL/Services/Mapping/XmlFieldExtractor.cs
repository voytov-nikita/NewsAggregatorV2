using System.Xml;
using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Services.Mapping;

/// <summary>
/// Applies a <see cref="FieldMapping"/> to an XML container node.
/// The <see cref="FieldMapping.Selector"/> is interpreted as an XPath expression
/// relative to the container.
/// </summary>
internal static class XmlFieldExtractor
{
    public static string? ExtractSingle(XmlNode container, FieldMapping? mapping)
    {
        if (mapping is null || string.IsNullOrWhiteSpace(mapping.Selector))
        {
            return null;
        }

        XmlNode? node = container.SelectSingleNode(mapping.Selector);
        if (node is null)
        {
            return null;
        }

        return ReadValue(node, mapping);
    }

    public static string[] ExtractMany(XmlNode container, FieldMapping? mapping)
    {
        if (mapping is null || string.IsNullOrWhiteSpace(mapping.Selector))
        {
            return Array.Empty<string>();
        }

        XmlNodeList? nodes = container.SelectNodes(mapping.Selector);
        if (nodes is null || nodes.Count == 0)
        {
            return Array.Empty<string>();
        }

        List<string> values = new(nodes.Count);
        foreach (XmlNode node in nodes)
        {
            string? value = ReadValue(node, mapping);
            if (!string.IsNullOrWhiteSpace(value))
            {
                values.Add(value);
            }
        }

        return values.ToArray();
    }

    private static string? ReadValue(XmlNode node, FieldMapping mapping)
    {
        if (mapping.From == FieldValueSource.Attribute)
        {
            if (string.IsNullOrEmpty(mapping.Attribute))
            {
                return null;
            }
            return node.Attributes?[mapping.Attribute]?.Value;
        }

        return node.InnerText;
    }
}
