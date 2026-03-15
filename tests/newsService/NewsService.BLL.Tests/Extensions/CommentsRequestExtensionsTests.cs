using AutoFixture;
using FluentAssertions;
using NewsService.API.Extensions.Comments;
using NewsService.API.Models.Comments;
using NewsService.Models.Comments.Models;

namespace NewsService.BLL.Tests.Extensions;

public class CommentsRequestExtensionsTests
{
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public void ToResponse_CommentsModel_ReturnsCommentsResponse()
    {
        var model = _fixture.Create<CommentsModel>();

        var result = model.ToResponse();

        result.Should().BeEquivalentTo(model, options => options
            .ExcludingMissingMembers());
    }

    [Fact]
    public void ToModel_CommentUpdateRequest_ReturnsCommentUpdateModel()
    {
        var request = _fixture.Create<CommentUpdateRequest>();
        var newsId = _fixture.Create<int>();
        var commentId = _fixture.Create<int>();

        var result = request.ToModel(newsId, commentId);

        result.NewsId.Should().Be(newsId);
        result.CommentId.Should().Be(commentId);
        result.Content.Should().Be(request.Content);
    }

    [Fact]
    public void ToModel_CommentCreateRequest_ReturnsCommentCreateModel()
    {
        var request = _fixture.Create<CommentCreateRequest>();
        var newsId = _fixture.Create<int>();

        var result = request.ToModel(newsId);

        result.NewsId.Should().Be(newsId);
        result.Content.Should().Be(request.Content);
    }

    [Fact]
    public void ToModel_CommentsFilterRequest_ReturnsCommentsFilterModel()
    {
        var request = _fixture.Create<CommentsFilterRequest>();

        var result = request.ToModel();

        result.Take.Should().Be(request.Take);
        result.Offset.Should().Be(request.Offset);
        result.OrderBy.Should().Be(request.OrderBy);
        result.OrderDirection.Should().Be(request.OrderDirection);
    }
}
