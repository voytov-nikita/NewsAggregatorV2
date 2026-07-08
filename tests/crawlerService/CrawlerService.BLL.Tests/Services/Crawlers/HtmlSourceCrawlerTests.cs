using Common.Models;
using CrawlerService.BLL.Services.Crawlers;
using CrawlerService.BLL.Tests.Helpers;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CrawlerService.BLL.Tests.Services.Crawlers;

public class HtmlSourceCrawlerTests
{
    private const string SampleHtml =
        """
        <html><body>
        <div class="news-item">
          <h2><a href="https://pub.test/1">Title 1</a></h2>
          <p class="desc">Description 1</p>
          <time datetime="2026-04-15T10:00:00Z">April 15</time>
          <span class="tag">news</span>
          <span class="tag">politics</span>
        </div>
        <div class="news-item">
          <h2><a href="https://pub.test/2">Title 2</a></h2>
          <p class="desc">Description 2</p>
        </div>
        </body></html>
        """;

    private static NewsSource BuildSource(HtmlSelectors? selectors) => new()
    {
        Id = "test",
        Name = "Test",
        PublisherName = "TestPub",
        PublisherLink = "https://pub.test",
        Url = "https://pub.test/news",
        Type = SourceType.Html,
        CronSchedule = "*/30 * * * * *",
        Category = NewsCategory.Backend,
        HtmlSelectors = selectors,
    };

    private static HtmlSelectors BuildSelectors() => new()
    {
        ItemSelector = "div.news-item",
        Title = new FieldMapping { Selector = "h2 a" },
        Link = new FieldMapping { Selector = "h2 a", From = FieldValueSource.Attribute, Attribute = "href" },
        Description = new FieldMapping { Selector = "p.desc" },
        PublishDate = new FieldMapping { Selector = "time", From = FieldValueSource.Attribute, Attribute = "datetime" },
    };

    [Fact]
    public async Task CrawlAsync_ValidHtml_ReturnsParsedItems()
    {
        NewsSource source = BuildSource(BuildSelectors());
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningString(SampleHtml);
        HtmlSourceCrawler sut = new(factory, NullLogger<HtmlSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Title 1");
        result[0].OriginalLink.Should().Be("https://pub.test/1");
        result[0].Description.Should().Be("Description 1");
        result[0].Guid.Should().Be("https://pub.test/1");
        result[0].GlobalUniqueId.Should().Be("testhttps://pub.test/1");
        result[0].SourceId.Should().Be("test");
        result[0].Category.Should().Be(NewsCategory.Backend);
        result[1].Title.Should().Be("Title 2");
        result[1].Category.Should().Be(NewsCategory.Backend);
    }

    [Fact]
    public async Task CrawlAsync_MissingSelectors_ReturnsEmpty()
    {
        NewsSource source = BuildSource(selectors: null);
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningString(SampleHtml);
        HtmlSourceCrawler sut = new(factory, NullLogger<HtmlSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CrawlAsync_NoItemsMatched_ReturnsEmpty()
    {
        HtmlSelectors selectors = BuildSelectors();
        selectors.ItemSelector = "div.nonexistent";
        NewsSource source = BuildSource(selectors);
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningString(SampleHtml);
        HtmlSourceCrawler sut = new(factory, NullLogger<HtmlSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CrawlAsync_HttpFailure_ReturnsEmpty()
    {
        NewsSource source = BuildSource(BuildSelectors());
        IHttpClientFactory factory = HttpClientFactoryStub.Throwing(new HttpRequestException("network down"));
        HtmlSourceCrawler sut = new(factory, NullLogger<HtmlSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().BeEmpty();
    }
}
