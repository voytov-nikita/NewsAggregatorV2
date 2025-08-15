using System.Linq.Expressions;
using AutoFixture;
using CrawlerService.BLL.Services;
using CrawlerService.Models.Models;
using FluentAssertions;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using Moq;
using Xunit;

namespace CrawlerService.BLL.Tests.Services;

public class NewsQueueServiceTests
{
    private readonly Mock<INewsProducer> _newsProducerMock = new();
    private readonly NewsQueueService _sut;
    private readonly Fixture _fixture = new Fixture();

    public NewsQueueServiceTests()
    {
        _sut = new NewsQueueService(
            _newsProducerMock.Object
        );
    }

    [Fact]
    public void AddManyToQueueAsync_NotEmptyData_Success()
    {
        // Arrange
        ParsedNews[] news = _fixture.CreateMany<ParsedNews>().ToArray();
        NewsQueueModel[] newsQueueModel = news.Select(_ => new NewsQueueModel
        {
            Title = _.Title,
            Description = _.Description,
            OriginalLink = _.OriginalLink,
            ImageLink = _.ImageLink,
            PublishDate = _.PublishDate,
            PublisherName = _.PublisherName,
            PublisherLink = _.PublisherLink,
            PublisherGuid = _.PublisherGuid,
            Guid = _.Guid
        }).ToArray();

        _newsProducerMock.Setup(x => x.Publish(newsQueueModel));

        // Act
        Func<Task> action = () => _sut.AddManyToQueueAsync(news);

        // Assert
        action.Should()
            .NotThrowAsync();
        
        _newsProducerMock.Verify(_ => _.Publish(It.Is(newsQueueModel.IsEqualTo())), Times.Once);
    }
    
    [Fact]
    public void AddManyToQueueAsync_EmptyData_Success()
    {
        // Arrange
        ParsedNews[] news = [];
        NewsQueueModel[] newsQueueModel = news.Select(_ => new NewsQueueModel
        {
            Title = _.Title,
            Description = _.Description,
            OriginalLink = _.OriginalLink,
            ImageLink = _.ImageLink,
            PublishDate = _.PublishDate,
            PublisherName = _.PublisherName,
            PublisherLink = _.PublisherLink,
            PublisherGuid = _.PublisherGuid,
            Guid = _.Guid
        }).ToArray();

        // Act
        Func<Task> action = () => _sut.AddManyToQueueAsync(news);

        // Assert
        action.Should()
            .NotThrowAsync();
        
        _newsProducerMock.Verify(_ => _.Publish(It.IsAny<NewsQueueModel[]>()), Times.Never);
    }
}