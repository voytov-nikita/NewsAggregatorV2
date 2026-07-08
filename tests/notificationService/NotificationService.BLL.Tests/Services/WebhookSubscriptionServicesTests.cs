using AutoFixture;
using Common.Models;
using FluentAssertions;
using Moq;
using NotificationService.BLL.Services;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Tests.Services;

public class WebhookSubscriptionServicesTests
{
    private readonly Mock<IWebhookSubscriptionsStore> _storeMock;
    private readonly WebhookSubscriptionServices _sut;
    private readonly Fixture _fixture = new();

    public WebhookSubscriptionServicesTests()
    {
        _storeMock = new Mock<IWebhookSubscriptionsStore>();
        _sut = new WebhookSubscriptionServices(_storeMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidArguments_ForwardsToStoreAndReturnsId()
    {
        var url = _fixture.Create<string>();
        var eventType = _fixture.Create<string>();
        var expectedId = _fixture.Create<string>();
        _storeMock.Setup(x => x.AddAsync(url, eventType)).ReturnsAsync(expectedId);

        var result = await _sut.AddAsync(url, eventType);

        result.Should().Be(expectedId);
        _storeMock.Verify(x => x.AddAsync(url, eventType), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_KnownId_ReturnsModel()
    {
        var id = _fixture.Create<string>();
        var expected = BuildModel();
        _storeMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(expected);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var id = _fixture.Create<string>();
        _storeMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((WebhookSubscriptionModel?)null);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetManyAsync_ForwardsFilterAndPaginationToStore()
    {
        var filter = new WebhooksFilter { Enabled = true, Actions = new[] { "news.created" } };
        var pagination = new OffsetPagination(5, 10);
        var expected = new OffsetCollection<WebhookSubscriptionModel>(
            new List<WebhookSubscriptionModel> { BuildModel() }, 5, 42);
        _storeMock.Setup(x => x.GetManyAsync(filter, pagination)).ReturnsAsync(expected);

        var result = await _sut.GetManyAsync(filter, pagination);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task UpdateAsync_ExistingSubscription_ReturnsTrue()
    {
        var id = _fixture.Create<string>();
        var url = _fixture.Create<string>();
        var eventType = _fixture.Create<string>();
        _storeMock.Setup(x => x.UpdateAsync(id, url, eventType, false)).ReturnsAsync(true);

        var result = await _sut.UpdateAsync(id, url, eventType, false);

        result.Should().BeTrue();
        _storeMock.Verify(x => x.UpdateAsync(id, url, eventType, false), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_MissingSubscription_ReturnsFalse()
    {
        var id = _fixture.Create<string>();
        _storeMock.Setup(x => x.UpdateAsync(id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(false);

        var result = await _sut.UpdateAsync(id, "u", "e", true);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingSubscription_ReturnsTrue()
    {
        var id = _fixture.Create<string>();
        _storeMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(id);

        result.Should().BeTrue();
        _storeMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_MissingSubscription_ReturnsFalse()
    {
        var id = _fixture.Create<string>();
        _storeMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(id);

        result.Should().BeFalse();
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
