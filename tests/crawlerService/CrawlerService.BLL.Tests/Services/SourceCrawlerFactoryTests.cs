using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrawlerService.BLL.Tests.Services;

public class SourceCrawlerFactoryTests
{
    [Fact]
    public void Get_KnownType_ReturnsMatchingCrawler()
    {
        Mock<ISourceCrawler> feedCrawler = new();
        feedCrawler.SetupGet(c => c.Type).Returns(SourceType.Feed);
        Mock<ISourceCrawler> htmlCrawler = new();
        htmlCrawler.SetupGet(c => c.Type).Returns(SourceType.Html);

        SourceCrawlerFactory sut = new(new[] { feedCrawler.Object, htmlCrawler.Object });

        ISourceCrawler result = sut.Get(SourceType.Html);

        result.Should().BeSameAs(htmlCrawler.Object);
    }

    [Fact]
    public void Get_UnknownType_Throws()
    {
        Mock<ISourceCrawler> feedCrawler = new();
        feedCrawler.SetupGet(c => c.Type).Returns(SourceType.Feed);

        SourceCrawlerFactory sut = new(new[] { feedCrawler.Object });

        Action act = () => sut.Get(SourceType.Html);

        act.Should().Throw<InvalidOperationException>();
    }
}
