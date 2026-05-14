using AutoFixture;
using FluentAssertions;
using Moq;
using NotificationService.BLL.Services;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;

namespace NotificationService.BLL.Tests.Services;

public class WebhookDeliveryAttemptServiceTests
{
    private readonly Mock<IWebhookDeliveryAttemptStore> _storeMock;
    private readonly WebhookDeliveryAttemptService _sut;
    private readonly Fixture _fixture = new();

    public WebhookDeliveryAttemptServiceTests()
    {
        _storeMock = new Mock<IWebhookDeliveryAttemptStore>();
        _sut = new WebhookDeliveryAttemptService(_storeMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidModel_ForwardsToStore()
    {
        var model = _fixture.Create<WebhookDeliveryAttemptModel>();

        Func<Task> act = () => _sut.AddAsync(model);

        await act.Should().NotThrowAsync();
        _storeMock.Verify(x => x.AddAsync(It.Is(model.IsEqualTo())), Times.Once);
    }
}
