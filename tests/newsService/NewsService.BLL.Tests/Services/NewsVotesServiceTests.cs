using AutoFixture;
using FluentAssertions;
using Moq;
using NewsService.BLL.Services;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.Votes.Models;

namespace NewsService.BLL.Tests.Services;

public class NewsVotesServiceTests
{
    private readonly Mock<INewsVotesStore> _newsVotesStoreMock = new();
    private readonly NewsVotesService _sut;
    private readonly Fixture _fixture = new();

    public NewsVotesServiceTests()
    {
        _sut = new NewsVotesService(_newsVotesStoreMock.Object);
    }

    [Theory]
    [InlineData((short)1)]
    [InlineData((short)-1)]
    [InlineData((short)0)]
    public async Task VoteAsync_AllowedValue_PassesTheVoteToTheStore(short value)
    {
        // Arrange
        var model = new NewsVoteModel { NewsId = 1, UserId = Guid.NewGuid(), Value = value };

        // Act
        Func<Task> act = () => _sut.VoteAsync(model);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _newsVotesStoreMock.Verify(_ => _.VoteAsync(It.Is(model.IsEqualTo())), Times.Once);
    }

    [Theory]
    [InlineData((short)2)]
    [InlineData((short)-5)]
    public async Task VoteAsync_ValueOutsideTheAllowedRange_ThrowsAndDoesNotWrite(short value)
    {
        // Arrange
        var model = new NewsVoteModel { NewsId = 1, UserId = Guid.NewGuid(), Value = value };

        // Act
        Func<Task> act = () => _sut.VoteAsync(model);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _newsVotesStoreMock.Verify(_ => _.VoteAsync(It.IsAny<NewsVoteModel>()), Times.Never);
    }

    [Fact]
    public async Task VoteAsync_Always_ReturnsTheRecountedTotalsFromTheStore()
    {
        // Likes/Dislikes on News are denormalized counters, so what the API reports back is the
        // store's recount rather than anything the caller supplied.

        // Arrange
        var model = new NewsVoteModel { NewsId = 1, UserId = Guid.NewGuid(), Value = 1 };
        var expected = _fixture.Create<NewsVoteResultModel>();
        _newsVotesStoreMock.Setup(_ => _.VoteAsync(It.IsAny<NewsVoteModel>())).ReturnsAsync(expected);

        // Act
        NewsVoteResultModel result = await _sut.VoteAsync(model);

        // Assert
        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task GetAsync_ValidData_Success()
    {
        // Arrange
        var newsId = _fixture.Create<int>();
        var userId = Guid.NewGuid();

        // Act
        Func<Task> act = () => _sut.GetAsync(newsId, userId);

        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _newsVotesStoreMock.Verify(_ => _.GetAsync(It.Is(newsId.IsEqualTo()), It.Is(userId.IsEqualTo())), Times.Once);
    }
}
