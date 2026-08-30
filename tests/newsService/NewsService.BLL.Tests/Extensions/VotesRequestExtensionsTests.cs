using AutoFixture;
using FluentAssertions;
using NewsService.API.Extensions.Votes;
using NewsService.API.Models.Votes;
using NewsService.Models.Votes.Models;

namespace NewsService.BLL.Tests.Extensions;

public class VotesRequestExtensionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToModel_NewsVoteRequest_TakesTheVoterFromTheParameterNotTheBody()
    {
        // Arrange
        var request = new NewsVoteRequest { Value = 1 };
        var newsId = _fixture.Create<int>();
        var userId = Guid.NewGuid();

        // Act
        NewsVoteModel result = request.ToModel(newsId, userId);

        // Assert
        result.NewsId.Should().Be(newsId);
        result.UserId.Should().Be(userId);
        result.Value.Should().Be(1);

        typeof(NewsVoteRequest).GetProperties().Select(_ => _.Name)
            .Should().NotContain(name => name.Contains("User", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ToResponse_NewsVoteResultModel_ReturnsNewsVoteResponse()
    {
        // Arrange
        var model = _fixture.Create<NewsVoteResultModel>();

        // Act
        NewsVoteResponse result = model.ToResponse();

        // Assert
        result.Should().BeEquivalentTo(model);
    }
}
