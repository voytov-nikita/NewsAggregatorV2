using AutoFixture;
using FluentAssertions;
using NewsService.API.Extensions.News;
using NewsService.API.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Tests.Extensions;

public class NewsRequestExtensionsTests
{
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public void ToResponse_NewsModel_ReturnsNewsResponse()
    {
        var model = _fixture.Create<NewsModel>();

        var result = model.ToResponse();

        result.Should().BeEquivalentTo(model, options => options
            .ExcludingMissingMembers());
    }

    [Fact]
    public void ToModel_NewsFilterRequest_ReturnsNewsFilterModel()
    {
        var request = _fixture.Create<NewsFilterRequest>();

        var result = request.ToModel();

        result.Take.Should().Be(request.Take);
        result.Offset.Should().Be(request.Offset);
        result.OrderBy.Should().Be(request.OrderBy);
        result.OrderDirection.Should().Be(request.OrderDirection);
        result.Keyword.Should().Be(request.Keyword);
    }
}
