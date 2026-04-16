using AngleSharp;
using AngleSharp.Dom;
using CrawlerService.BLL.Services.Mapping;
using CrawlerService.Models.Models;
using FluentAssertions;
using Xunit;

namespace CrawlerService.BLL.Tests.Services.Mapping;

public class HtmlFieldExtractorTests
{
    private static async Task<IElement> LoadContainerAsync(string html)
    {
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(req => req.Content(html));
        return document.Body!;
    }

    [Fact]
    public async Task ExtractSingle_TextSelector_ReturnsTextContent()
    {
        IElement container = await LoadContainerAsync("<div><h2>Hello</h2></div>");
        FieldMapping mapping = new() { Selector = "h2" };

        string? result = HtmlFieldExtractor.ExtractSingle(container, mapping);

        result.Should().Be("Hello");
    }

    [Fact]
    public async Task ExtractSingle_AttributeSelector_ReturnsAttribute()
    {
        IElement container = await LoadContainerAsync("""<div><a href="https://x.test">link</a></div>""");
        FieldMapping mapping = new() { Selector = "a", From = FieldValueSource.Attribute, Attribute = "href" };

        string? result = HtmlFieldExtractor.ExtractSingle(container, mapping);

        result.Should().Be("https://x.test");
    }

    [Fact]
    public async Task ExtractSingle_NullMapping_ReturnsNull()
    {
        IElement container = await LoadContainerAsync("<div><h2>Hello</h2></div>");

        string? result = HtmlFieldExtractor.ExtractSingle(container, mapping: null);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExtractSingle_SelectorNoMatch_ReturnsNull()
    {
        IElement container = await LoadContainerAsync("<div><h2>Hello</h2></div>");
        FieldMapping mapping = new() { Selector = "span.missing" };

        string? result = HtmlFieldExtractor.ExtractSingle(container, mapping);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExtractSingle_AttributeModeWithoutAttributeName_ReturnsNull()
    {
        IElement container = await LoadContainerAsync("""<div><a href="https://x.test">link</a></div>""");
        FieldMapping mapping = new() { Selector = "a", From = FieldValueSource.Attribute };

        string? result = HtmlFieldExtractor.ExtractSingle(container, mapping);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExtractMany_TextSelector_ReturnsAllValues()
    {
        IElement container = await LoadContainerAsync(
            "<div><span class='t'>a</span><span class='t'>b</span></div>");
        FieldMapping mapping = new() { Selector = "span.t" };

        string[] result = HtmlFieldExtractor.ExtractMany(container, mapping);

        result.Should().BeEquivalentTo(new[] { "a", "b" });
    }

    [Fact]
    public async Task ExtractMany_NoMatches_ReturnsEmpty()
    {
        IElement container = await LoadContainerAsync("<div><h2>Hello</h2></div>");
        FieldMapping mapping = new() { Selector = "span.t" };

        string[] result = HtmlFieldExtractor.ExtractMany(container, mapping);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractMany_NullMapping_ReturnsEmpty()
    {
        IElement container = await LoadContainerAsync("<div><span class='t'>a</span></div>");

        string[] result = HtmlFieldExtractor.ExtractMany(container, mapping: null);

        result.Should().BeEmpty();
    }
}
