using System.Text;
using Common.Models;
using CrawlerService.BLL.Services.Crawlers;
using CrawlerService.BLL.Tests.Helpers;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CrawlerService.BLL.Tests.Services.Crawlers;

public class FeedSourceCrawlerTests
{
    private static NewsSource BuildSource(FeedFieldMapping? mapping = null) => new()
    {
        Id = "test",
        Name = "Test",
        PublisherName = "TestPub",
        PublisherLink = "https://pub.test",
        Url = "https://pub.test/rss",
        Type = SourceType.Feed,
        CronSchedule = "*/30 * * * * *",
        Category = NewsCategory.Frontend,
        FeedMapping = mapping,
    };

    private const string SampleRss =
        """
        <?xml version="1.0" encoding="UTF-8"?>
        <rss version="2.0"><channel>
          <title>Test Channel</title>
          <link>https://pub.test</link>
          <description>Test</description>
          <item>
            <title>Item 1</title>
            <description>Desc 1</description>
            <link>https://pub.test/1</link>
            <guid>guid-1</guid>
            <pubDate>Wed, 15 Apr 2026 10:00:00 GMT</pubDate>
            <category>news</category>
            <category>politics</category>
          </item>
          <item>
            <title>Item 2</title>
            <description>Desc 2</description>
            <link>https://pub.test/2</link>
            <guid>guid-2</guid>
            <pubDate>Thu, 16 Apr 2026 11:00:00 GMT</pubDate>
          </item>
        </channel></rss>
        """;

    [Fact]
    public async Task CrawlAsync_SyndicationPath_ReturnsParsedItems()
    {
        NewsSource source = BuildSource();
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningBytes(Encoding.UTF8.GetBytes(SampleRss));
        FeedSourceCrawler sut = new(factory, NullLogger<FeedSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Item 1");
        result[0].Description.Should().Be("Desc 1");
        result[0].OriginalLink.Should().Be("https://pub.test/1");
        result[0].Guid.Should().Be("guid-1");
        result[0].SourceId.Should().Be("test");
        result[0].PublisherName.Should().Be("TestPub");
        result[0].GlobalUniqueId.Should().Be("testguid-1");
        result[0].Category.Should().Be(NewsCategory.Frontend);
        result[1].Title.Should().Be("Item 2");
        result[1].Category.Should().Be(NewsCategory.Frontend);
    }

    [Fact]
    public async Task CrawlAsync_XPathPath_ReturnsParsedItems()
    {
        FeedFieldMapping mapping = new()
        {
            ItemXPath = "//item",
            Title = new FieldMapping { Selector = "title" },
            Description = new FieldMapping { Selector = "description" },
            Link = new FieldMapping { Selector = "link" },
            Guid = new FieldMapping { Selector = "guid" },
            PublishDate = new FieldMapping { Selector = "pubDate" },
        };
        NewsSource source = BuildSource(mapping);
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningBytes(Encoding.UTF8.GetBytes(SampleRss));
        FeedSourceCrawler sut = new(factory, NullLogger<FeedSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Item 1");
        result[0].Guid.Should().Be("guid-1");
        result[0].OriginalLink.Should().Be("https://pub.test/1");
        result[0].GlobalUniqueId.Should().Be("testguid-1");
        result[0].Category.Should().Be(NewsCategory.Frontend);
    }

    [Fact]
    public async Task CrawlAsync_HttpFailure_ReturnsEmpty()
    {
        NewsSource source = BuildSource();
        IHttpClientFactory factory = HttpClientFactoryStub.Throwing(new HttpRequestException("network down"));
        FeedSourceCrawler sut = new(factory, NullLogger<FeedSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CrawlAsync_MalformedXml_ReturnsEmpty()
    {
        NewsSource source = BuildSource();
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningBytes(Encoding.UTF8.GetBytes("not-xml"));
        FeedSourceCrawler sut = new(factory, NullLogger<FeedSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CrawlAsync_XPathNoMatches_ReturnsEmpty()
    {
        FeedFieldMapping mapping = new()
        {
            ItemXPath = "//nonexistent",
            Title = new FieldMapping { Selector = "title" },
            Link = new FieldMapping { Selector = "link" },
            Guid = new FieldMapping { Selector = "guid" },
        };
        NewsSource source = BuildSource(mapping);
        IHttpClientFactory factory = HttpClientFactoryStub.ReturningBytes(Encoding.UTF8.GetBytes(SampleRss));
        FeedSourceCrawler sut = new(factory, NullLogger<FeedSourceCrawler>.Instance);

        ParsedNews[] result = await sut.CrawlAsync(source);

        result.Should().BeEmpty();
    }
}
