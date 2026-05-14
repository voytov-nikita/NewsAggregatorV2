using AutoFixture;
using FluentAssertions;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.BLL.Abstractions.Services;
using NotificationService.Models.Webhooks;
using NotificationService.Services;

namespace NotificationService.BLL.Tests.Services;

public class WebhookTriggeredProcessorTests
{
    private readonly Mock<IWebhookTriggeredConsumer> _consumerMock;
    private readonly Mock<IWebhookDeliveryService> _deliveryServiceMock;
    private readonly Mock<ILogger<WebhookTriggeredProcessor>> _loggerMock;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WebhookTriggeredProcessor _sut;
    private readonly Fixture _fixture = new();

    public WebhookTriggeredProcessorTests()
    {
        _consumerMock = new Mock<IWebhookTriggeredConsumer>();
        _deliveryServiceMock = new Mock<IWebhookDeliveryService>();
        _loggerMock = new Mock<ILogger<WebhookTriggeredProcessor>>();

        var services = new ServiceCollection();
        services.AddSingleton(_deliveryServiceMock.Object);
        _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

        _sut = new WebhookTriggeredProcessor(_consumerMock.Object, _scopeFactory, _loggerMock.Object);
    }

    [Fact]
    public async Task StartAsync_OnStart_AddsSubscriptionToConsumer()
    {
        await _sut.StartAsync(CancellationToken.None);

        _consumerMock.Verify(x => x.AddSubscription(It.IsAny<Func<WebhookTriggeredQueueModel, Task>>()), Times.Once);
    }

    [Fact]
    public async Task StartAsync_ConsumerThrows_LogsAndRethrows()
    {
        var error = new InvalidOperationException("boom");
        _consumerMock.Setup(x => x.AddSubscription(It.IsAny<Func<WebhookTriggeredQueueModel, Task>>()))
            .Throws(error);

        Func<Task> act = () => _sut.StartAsync(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Process_ValidMessage_DeliversMappedDispatchModel()
    {
        Func<WebhookTriggeredQueueModel, Task>? captured = null;
        _consumerMock.Setup(x => x.AddSubscription(It.IsAny<Func<WebhookTriggeredQueueModel, Task>>()))
            .Callback<Func<WebhookTriggeredQueueModel, Task>>(cb => captured = cb);

        await _sut.StartAsync(CancellationToken.None);

        var message = new WebhookTriggeredQueueModel
        {
            Url = _fixture.Create<string>(),
            SubscriptionId = _fixture.Create<string>(),
            EventType = _fixture.Create<string>(),
            Data = new { payload = "value" }
        };

        captured.Should().NotBeNull();
        await captured!(message);

        _deliveryServiceMock.Verify(x => x.DeliverAsync(It.Is<WebhookDispatchModel>(m =>
            m.Url == message.Url
            && m.SubscriptionId == message.SubscriptionId
            && m.EventType == message.EventType
            && m.Payload == message.Data)), Times.Once);
    }
}
