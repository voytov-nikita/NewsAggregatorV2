using System.Linq.Expressions;
using AutoFixture;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CrawlerService.BLL.Tests.Services;

public class NewsServiceTests
{
    private readonly Mock<INewsStore> _newsStoreMock = new();
    private readonly Mock<ISourceStore> _sourceStoreMock = new();
    private readonly Mock<IPostponedJobRunner> _postponedJobRunnerMock = new();
    private readonly NewsService _sut;
    private readonly Fixture _fixture = new Fixture();

    public NewsServiceTests()
    {
        _sut = new NewsService(
            _newsStoreMock.Object,
            _sourceStoreMock.Object,
            _postponedJobRunnerMock.Object,
            NullLogger<NewsService>.Instance
        );
    }

    [Fact]
    public async Task SaveUniqueNewsAsync_NoNewsExists_NewNewsInsertedAndPassedToQueueService()
    {
        // Arrange
        ParsedNews[] parsedNews = _fixture.Build<ParsedNews>()
            .With(n => n.GlobalUniqueId, () => Guid.NewGuid().ToString())
            .CreateMany()
            .ToArray();
        
        string[] globalUniqueIds = parsedNews.Select(_ => _.GlobalUniqueId).ToArray();
        
        _newsStoreMock
            .Setup(x => x.ExcludeExistedAsync(globalUniqueIds))
            .ReturnsAsync(globalUniqueIds);

        // Act
        Func<Task> action = () => _sut.SaveUniqueNewsAsync(parsedNews);

        // Assert
        await action.Should()
            .NotThrowAsync();

        _newsStoreMock.Verify(x => x.InsertAsync(parsedNews), Times.Once);

        _postponedJobRunnerMock.Verify(
            x => x.Enqueue(It.IsAny<Expression<Func<INewsQueueService, Task>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveUniqueNewsAsync_SomeNewsNotExists_NewNewsInsertedAndPassedToQueueService()
    {
        // Arrange
        ParsedNews[] notExistsNews = _fixture.Build<ParsedNews>()
            .With(n => n.GlobalUniqueId, () => Guid.NewGuid().ToString())
            .CreateMany()
            .ToArray();

        ParsedNews[] existsNews = _fixture.Build<ParsedNews>()
            .With(n => n.GlobalUniqueId, () => Guid.NewGuid().ToString())
            .CreateMany()
            .ToArray();

        ParsedNews[] allNews = notExistsNews.Concat(existsNews).ToArray();

        string[] notExistsNewsIds = notExistsNews.Select(_ => _.GlobalUniqueId).ToArray();
        string[] allNewsNewsIds = allNews.Select(_ => _.GlobalUniqueId).ToArray();
        _newsStoreMock
            .Setup(x => x.ExcludeExistedAsync(It.Is(allNewsNewsIds.IsEqualTo())))
            .ReturnsAsync(notExistsNewsIds);

        // Act
        Func<Task> action = () => _sut.SaveUniqueNewsAsync(allNews);

        // Assert
        await action.Should()
            .NotThrowAsync();

        _newsStoreMock.Verify(x => x.InsertAsync(notExistsNews), Times.Once);

        _postponedJobRunnerMock.Verify(x =>
                x.Enqueue(It.IsAny<Expression<Func<INewsQueueService, Task>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveUniqueNewsAsync_AllNewsExists_NoNewsInsertedAndPassedToQueueService()
    {
        // Arrange
        ParsedNews[] parsedNews = _fixture.Build<ParsedNews>()
            .With(n => n.GlobalUniqueId, () => Guid.NewGuid().ToString())
            .CreateMany()
            .ToArray();

        
        string[] globalUniqueIds = parsedNews.Select(_ => _.GlobalUniqueId).ToArray();
        _newsStoreMock
            .Setup(x => x.ExcludeExistedAsync(It.Is(globalUniqueIds.IsEqualTo())))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        Func<Task> action = () => _sut.SaveUniqueNewsAsync(parsedNews);

        // Assert
        await action.Should()
            .NotThrowAsync();

        _newsStoreMock.Verify(x => x.InsertAsync(It.IsAny<ParsedNews[]>()), Times.Never);

        _postponedJobRunnerMock.Verify(x =>
                x.Enqueue(It.IsAny<Expression<Func<INewsQueueService, Task>>>()),
            Times.Never
        );
    }

    [Fact]
    public async Task SaveUniqueNewsAsync_EmptyNews_NoExceptionThrown()
    {
        // Arrange
        ParsedNews[] parsedNews = [];
        
        _newsStoreMock
            .Setup(x => x.ExcludeExistedAsync(Array.Empty<string>()))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        Func<Task> action = () => _sut.SaveUniqueNewsAsync(parsedNews);

        // Assert
        await action.Should()
            .NotThrowAsync();

        _newsStoreMock.Verify(x => x.InsertAsync(It.IsAny<ParsedNews[]>()), Times.Never);

        _postponedJobRunnerMock.Verify(x =>
                x.Enqueue(It.IsAny<Expression<Func<INewsQueueService, Task>>>()),
            Times.Never
        );
    }
}