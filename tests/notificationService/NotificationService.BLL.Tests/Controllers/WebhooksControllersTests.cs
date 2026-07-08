using AutoFixture;
using Common.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationService.BLL.Abstractions.Services;
using NotificationService.Controllers;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Tests.Controllers;

public class WebhooksControllersTests
{
    private readonly Mock<IWebhookSubscriptionsServices> _serviceMock = new();
    private readonly WebhooksControllers _sut;
    private readonly Fixture _fixture = new();

    public WebhooksControllersTests()
    {
        _sut = new WebhooksControllers(_serviceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };
    }

    [Fact]
    public async Task GetMany_MapsFilterAndPaginationAndAddsHeaders()
    {
        WebhookFilterRequest request = new()
        {
            Actions = new[] { "news.created" },
            Enabled = true,
            Offset = 5,
            Take = 20,
        };

        WebhookSubscriptionModel model = BuildModel();
        OffsetCollection<WebhookSubscriptionModel> collection =
            new(new List<WebhookSubscriptionModel> { model }, 5, 42);

        _serviceMock
            .Setup(x => x.GetManyAsync(
                It.Is<WebhooksFilter>(f => f.Enabled == true && f.Actions!.Single() == "news.created"),
                It.Is<OffsetPagination>(p => p.Offset == 5 && p.Take == 20)))
            .ReturnsAsync(collection);

        List<WebhookSubscriptionModel> result = await _sut.GetMany(request);

        result.Should().ContainSingle().Which.Should().BeSameAs(model);
        _sut.Response.Headers.Should().ContainKey("X-Pagination-Total");
        _sut.Response.Headers["X-Pagination-Total"].ToString().Should().Be("42");
    }

    [Fact]
    public async Task GetMany_NegativeOffset_ClampsToZero()
    {
        WebhookFilterRequest request = new() { Offset = -5, Take = 0 };
        OffsetCollection<WebhookSubscriptionModel> empty =
            new(new List<WebhookSubscriptionModel>(), 0, 0);

        _serviceMock
            .Setup(x => x.GetManyAsync(
                It.IsAny<WebhooksFilter>(),
                It.Is<OffsetPagination>(p => p.Offset == 0 && p.Take == 100)))
            .ReturnsAsync(empty);

        await _sut.GetMany(request);

        _serviceMock.Verify(x => x.GetManyAsync(
            It.IsAny<WebhooksFilter>(),
            It.Is<OffsetPagination>(p => p.Offset == 0 && p.Take == 100)), Times.Once);
    }

    [Fact]
    public async Task GetById_Found_ReturnsOk()
    {
        WebhookSubscriptionModel model = BuildModel();
        _serviceMock.Setup(x => x.GetByIdAsync(model.Id)).ReturnsAsync(model);

        ActionResult<WebhookSubscriptionModel> result = await _sut.GetById(model.Id);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(model);
    }

    [Fact]
    public async Task GetById_Missing_ReturnsNotFound()
    {
        _serviceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((WebhookSubscriptionModel?)null);

        ActionResult<WebhookSubscriptionModel> result = await _sut.GetById("abc");

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Subscribe_ValidRequest_ReturnsCreated()
    {
        WebhookCreateRequest request = new() { Url = "https://example.com", Event = "news.created" };
        _serviceMock.Setup(x => x.AddAsync(request.Url, request.Event)).ReturnsAsync("507f1f77bcf86cd799439011");

        ActionResult<string> result = await _sut.Subscribe(request);

        result.Result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().Be("507f1f77bcf86cd799439011");
    }

    [Theory]
    [InlineData(null, "event")]
    [InlineData("", "event")]
    [InlineData("url", null)]
    [InlineData("url", "")]
    public async Task Subscribe_InvalidRequest_ReturnsBadRequest(string? url, string? evt)
    {
        WebhookCreateRequest request = new() { Url = url!, Event = evt! };

        ActionResult<string> result = await _sut.Subscribe(request);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _serviceMock.Verify(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Update_ExistingSubscription_ReturnsNoContent()
    {
        WebhookUpdateRequest request = new() { Url = "https://x", Event = "e", Enabled = false };
        _serviceMock.Setup(x => x.UpdateAsync("id", request.Url, request.Event, request.Enabled))
            .ReturnsAsync(true);

        IActionResult result = await _sut.Update("id", request);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_MissingSubscription_ReturnsNotFound()
    {
        WebhookUpdateRequest request = new() { Url = "https://x", Event = "e", Enabled = true };
        _serviceMock.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(false);

        IActionResult result = await _sut.Update("id", request);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_EmptyFields_ReturnsBadRequest()
    {
        WebhookUpdateRequest request = new() { Url = "", Event = "e", Enabled = true };

        IActionResult result = await _sut.Update("id", request);

        result.Should().BeOfType<BadRequestObjectResult>();
        _serviceMock.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ExistingSubscription_ReturnsNoContent()
    {
        _serviceMock.Setup(x => x.DeleteAsync("id")).ReturnsAsync(true);

        IActionResult result = await _sut.Delete("id");

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_MissingSubscription_ReturnsNotFound()
    {
        _serviceMock.Setup(x => x.DeleteAsync(It.IsAny<string>())).ReturnsAsync(false);

        IActionResult result = await _sut.Delete("id");

        result.Should().BeOfType<NotFoundResult>();
    }

    private WebhookSubscriptionModel BuildModel() => new WebhookSubscriptionModel
    {
        Id = _fixture.Create<string>(),
        Url = _fixture.Create<string>(),
        Action = _fixture.Create<string>(),
        CreationTime = DateTime.UtcNow,
        Enabled = true,
    };
}
