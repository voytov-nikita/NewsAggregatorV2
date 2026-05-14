using AutoFixture;
using FluentAssertions;
using Moq;
using NotificationService.BLL.Services;
using NotificationService.DAL.Abstractions.Stores;

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
    public async Task AddAsync_ValidArguments_ForwardsToStore()
    {
        var url = _fixture.Create<string>();
        var eventType = _fixture.Create<string>();

        Func<Task> act = () => _sut.AddAsync(url, eventType);

        await act.Should().NotThrowAsync();
        _storeMock.Verify(x => x.AddAsync(url, eventType), Times.Once);
    }
}
