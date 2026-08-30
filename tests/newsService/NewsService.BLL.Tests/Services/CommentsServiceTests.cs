using AutoFixture;
using Common.Auth.Constants;
using Common.Auth.Services;
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
    private Mock<ICurrentUser> _currentUserMock;
    private CommentsService _sut;
    private Fixture _fixture = new Fixture();
    private readonly Guid _authorId = Guid.NewGuid();

    public CommentsServiceTests()
    {
        _commentsStoreMock = new Mock<ICommentsStore>();
        _currentUserMock = new Mock<ICurrentUser>();
        _sut = new CommentsService(_commentsStoreMock.Object, _currentUserMock.Object);
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
    public async Task UpdateAsync_CallerIsTheAuthor_Success()
    {
        // Arrange
        var updateModel = _fixture.Create<CommentUpdateModel>();
        SetupCaller(_authorId, canModerate: false);
        SetupStoredAuthor(updateModel.NewsId, updateModel.CommentId, _authorId);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(updateModel);

        // Assert
        await act.Should()
            .NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.UpdateAsync(It.Is(updateModel.IsEqualTo())), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallerIsNotTheAuthorAndCannotModerate_ThrowsAndDoesNotWrite()
    {
        // Arrange
        var updateModel = _fixture.Create<CommentUpdateModel>();
        SetupCaller(Guid.NewGuid(), canModerate: false);
        SetupStoredAuthor(updateModel.NewsId, updateModel.CommentId, _authorId);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(updateModel);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _commentsStoreMock.Verify(x => x.UpdateAsync(It.IsAny<CommentUpdateModel>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_CallerCanModerate_SucceedsOnAnotherUsersComment()
    {
        // A moderator never even needs the stored author looked up.

        // Arrange
        var updateModel = _fixture.Create<CommentUpdateModel>();
        SetupCaller(Guid.NewGuid(), canModerate: true);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(updateModel);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.UpdateAsync(It.Is(updateModel.IsEqualTo())), Times.Once);
        _commentsStoreMock.Verify(x => x.GetAuthorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_CommentDoesNotExist_ThrowsKeyNotFound()
    {
        // Arrange
        var updateModel = _fixture.Create<CommentUpdateModel>();
        SetupCaller(_authorId, canModerate: false);
        _commentsStoreMock
            .Setup(x => x.GetAuthorIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Guid?)null);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(updateModel);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
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
    public async Task DeleteAsync_CallerIsTheAuthor_Success()
    {
        // Arrange
        var newsId = _fixture.Create<int>();
        var commentId = _fixture.Create<int>();
        SetupCaller(_authorId, canModerate: false);
        SetupStoredAuthor(newsId, commentId, _authorId);

        // Act
        Func<Task> act = () => _sut.DeleteAsync(newsId, commentId);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _commentsStoreMock.Verify(x => x.DeleteAsync(It.Is(newsId.IsEqualTo()), It.Is(commentId.IsEqualTo())),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallerIsNotTheAuthorAndCannotModerate_ThrowsAndDoesNotDelete()
    {
        // Arrange
        var newsId = _fixture.Create<int>();
        var commentId = _fixture.Create<int>();
        SetupCaller(Guid.NewGuid(), canModerate: false);
        SetupStoredAuthor(newsId, commentId, _authorId);

        // Act
        Func<Task> act = () => _sut.DeleteAsync(newsId, commentId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _commentsStoreMock.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    private void SetupCaller(Guid userId, bool canModerate)
    {
        _currentUserMock.SetupGet(_ => _.UserId).Returns(userId);
        _currentUserMock.Setup(_ => _.HasPermission(Permissions.CommentModerate)).Returns(canModerate);
    }

    private void SetupStoredAuthor(int newsId, int commentId, Guid authorId) =>
        _commentsStoreMock
            .Setup(x => x.GetAuthorIdAsync(newsId, commentId))
            .ReturnsAsync(authorId);
}
