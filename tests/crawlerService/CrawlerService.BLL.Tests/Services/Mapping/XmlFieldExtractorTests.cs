using System.Xml;
using CrawlerService.BLL.Services.Mapping;
using CrawlerService.Models.Models;
using FluentAssertions;
using Xunit;

namespace CrawlerService.BLL.Tests.Services.Mapping;

public class XmlFieldExtractorTests
{
    private static XmlNode LoadItem(string xml)
    {
        XmlDocument doc = new();
        doc.LoadXml(xml);
        return doc.DocumentElement!;
    }

    [Fact]
    public void ExtractSingle_TextSelector_ReturnsInnerText()
    {
        XmlNode node = LoadItem("<item><title>Hello</title></item>");
        FieldMapping mapping = new() { Selector = "title" };

        string? result = XmlFieldExtractor.ExtractSingle(node, mapping);

        result.Should().Be("Hello");
    }

    [Fact]
    public void ExtractSingle_AttributeSelector_ReturnsAttributeValue()
    {
        XmlNode node = LoadItem("""<item><link href="https://x.test/1" /></item>""");
        FieldMapping mapping = new() { Selector = "link", From = FieldValueSource.Attribute, Attribute = "href" };

        string? result = XmlFieldExtractor.ExtractSingle(node, mapping);

        result.Should().Be("https://x.test/1");
    }

    [Fact]
    public void ExtractSingle_NullMapping_ReturnsNull()
    {
        XmlNode node = LoadItem("<item><title>Hello</title></item>");

        string? result = XmlFieldExtractor.ExtractSingle(node, mapping: null);

        result.Should().BeNull();
    }

    [Fact]
    public void ExtractSingle_SelectorNoMatch_ReturnsNull()
    {
        XmlNode node = LoadItem("<item><title>Hello</title></item>");
        FieldMapping mapping = new() { Selector = "missing" };

        string? result = XmlFieldExtractor.ExtractSingle(node, mapping);

        result.Should().BeNull();
    }

    [Fact]
    public void ExtractSingle_AttributeModeWithoutAttributeName_ReturnsNull()
    {
        XmlNode node = LoadItem("""<item><link href="https://x.test/1" /></item>""");
        FieldMapping mapping = new() { Selector = "link", From = FieldValueSource.Attribute };

        string? result = XmlFieldExtractor.ExtractSingle(node, mapping);

        result.Should().BeNull();
    }

    [Fact]
    public void ExtractMany_TextSelector_ReturnsAllValues()
    {
        XmlNode node = LoadItem("<item><category>a</category><category>b</category></item>");
        FieldMapping mapping = new() { Selector = "category" };

        string[] result = XmlFieldExtractor.ExtractMany(node, mapping);

        result.Should().BeEquivalentTo(new[] { "a", "b" });
    }

    [Fact]
    public void ExtractMany_NoMatches_ReturnsEmpty()
    {
        XmlNode node = LoadItem("<item><title>Hello</title></item>");
        FieldMapping mapping = new() { Selector = "category" };

        string[] result = XmlFieldExtractor.ExtractMany(node, mapping);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractMany_NullMapping_ReturnsEmpty()
    {
        XmlNode node = LoadItem("<item><category>a</category></item>");

        string[] result = XmlFieldExtractor.ExtractMany(node, mapping: null);

        result.Should().BeEmpty();
    }
}
