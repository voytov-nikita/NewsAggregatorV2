using AutoFixture;
using FluentAssertions;
using Moq;
using NewsService.BLL.Services;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.Comments.Enums;
using NewsService.Models.Comments.Models;

namespace NewsService.BLL.Tests.Services;

public class CommentsServiceTests
{
    private Mock<ICommentsStore> _commentsStoreMock;
    private CommentsService _sut;
    private Fixture _fixture = new Fixture();

    public CommentsServiceTests()
    {
        _commentsStoreMock = new Mock<ICommentsStore>();
        _sut = new CommentsService(_commentsStoreMock.Object);
    }

    [Fact]
    public async Task GetManyAsync_FilterAndNewsIdValid_Success()
    {
        // Arrange
        var filter = _fixture.Create<CommentsFilterModel>();
        var newsId = _fixture.Create<int>();

        // Act
        Func<Task> act = () => _sut.GetManyAsync(filter, newsId);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.GetManyAsync(It.Is(filter.IsEqualTo()), It.Is(newsId.IsEqualTo())),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidData_Success()
    {
        // Arrange
        var updateModel = _fixture.Create<CommentUpdateModel>();

        // Act
        Func<Task> act = () => _sut.UpdateAsync(updateModel);

        // Assert
        await act.Should()
            .NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.UpdateAsync(It.Is(updateModel.IsEqualTo())), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidData_Success()
    {
        // Arrange
        var createModel = new CommentCreateModel();

        // Act
        Func<Task> act = () => _sut.CreateAsync(createModel);

        // Assert
        await act.Should()
            .NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.CreateAsync(It.Is(createModel.IsEqualTo())), Times.Once);
    }

    [Fact]
    public async Task RateAsync_ValidData_Success()
    {
        // Arrange
        var newsId = _fixture.Create<int>();
        var commentId = _fixture.Create<int>();
        var rateType = _fixture.Create<RateType>();

        // Act
        Func<Task> act = () => _sut.RateAsync(newsId, commentId, rateType);

        // Assert
        await act.Should()
            .NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(
            x => x.RateAsync(It.Is(newsId.IsEqualTo()), It.Is(commentId.IsEqualTo()), It.Is(rateType.IsEqualTo())),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidData_Success()
    {
        // Arrange
        var newsId = _fixture.Create<int>();
        var commentId = _fixture.Create<int>();

        // Act
        Func<Task> act = () => _sut.DeleteAsync(newsId, commentId);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.DeleteAsync(It.Is(newsId.IsEqualTo()), It.Is(commentId.IsEqualTo())),
            Times.Once);
    }
}