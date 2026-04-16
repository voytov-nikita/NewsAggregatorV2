using System.Linq.Expressions;
using AutoFixture;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CrawlerService.BLL.Tests.Services;

public class NewsDownloaderServiceTests
{
    private readonly Mock<ISourceStore> _sourceStoreMock = new();
    private readonly Mock<ISourceCrawlerFactory> _crawlerFactoryMock = new();
    private readonly Mock<ISourceCrawler> _crawlerMock = new();
    private readonly Mock<IPostponedJobRunner> _postponedJobRunnerMock = new();
    private readonly NewsDownloaderService _sut;
    private readonly Fixture _fixture = new();

    public NewsDownloaderServiceTests()
    {
        _sut = new NewsDownloaderService(
            _sourceStoreMock.Object,
            _crawlerFactoryMock.Object,
            _postponedJobRunnerMock.Object,
            NullLogger<NewsDownloaderService>.Instance
        );
    }

    [Fact]
    public async Task GetNewsAsync_SourceNotFound_DoesNothing()
    {
        _sourceStoreMock.Setup(x => x.GetByIdAsync("missing")).ReturnsAsync((NewsSource?)null);

        await _sut.GetNewsAsync("missing");

        _crawlerFactoryMock.Verify(x => x.Get(It.IsAny<SourceType>()), Times.Never);
        _postponedJobRunnerMock.Verify(
            x => x.Enqueue(It.IsAny<Expression<Func<INewsService, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetNewsAsync_SourceDisabled_DoesNothing()
    {
        NewsSource source = BuildSource(enabled: false);
        _sourceStoreMock.Setup(x => x.GetByIdAsync(source.Id)).ReturnsAsync(source);

        await _sut.GetNewsAsync(source.Id);

        _crawlerFactoryMock.Verify(x => x.Get(It.IsAny<SourceType>()), Times.Never);
        _postponedJobRunnerMock.Verify(
            x => x.Enqueue(It.IsAny<Expression<Func<INewsService, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetNewsAsync_CrawlerReturnsEmpty_NothingEnqueued()
    {
        NewsSource source = BuildSource(enabled: true);
        _sourceStoreMock.Setup(x => x.GetByIdAsync(source.Id)).ReturnsAsync(source);
        _crawlerFactoryMock.Setup(x => x.Get(source.Type)).Returns(_crawlerMock.Object);
        _crawlerMock.Setup(x => x.CrawlAsync(source)).ReturnsAsync(Array.Empty<ParsedNews>());

        await _sut.GetNewsAsync(source.Id);

        _postponedJobRunnerMock.Verify(
            x => x.Enqueue(It.IsAny<Expression<Func<INewsService, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetNewsAsync_CrawlerReturnsItems_EnqueuesSaveJob()
    {
        NewsSource source = BuildSource(enabled: true);
        ParsedNews[] items = _fixture.CreateMany<ParsedNews>(3).ToArray();
        _sourceStoreMock.Setup(x => x.GetByIdAsync(source.Id)).ReturnsAsync(source);
        _crawlerFactoryMock.Setup(x => x.Get(source.Type)).Returns(_crawlerMock.Object);
        _crawlerMock.Setup(x => x.CrawlAsync(source)).ReturnsAsync(items);

        await _sut.GetNewsAsync(source.Id);

        _postponedJobRunnerMock.Verify(
            x => x.Enqueue(It.IsAny<Expression<Func<INewsService, Task>>>()),
            Times.Once);
    }

    private static NewsSource BuildSource(bool enabled) => new()
    {
        Id = "test",
        Name = "Test",
        PublisherName = "TestPub",
        PublisherLink = "https://pub.test",
        Url = "https://pub.test/rss",
        Type = SourceType.Feed,
        CronSchedule = "*/30 * * * * *",
        Enabled = enabled,
    };
}
