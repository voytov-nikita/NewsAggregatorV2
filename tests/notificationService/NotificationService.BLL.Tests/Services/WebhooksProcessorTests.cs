using AutoFixture;
using Common.Models;
using FluentAssertions;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.DAL.Abstractions.Stores;
using NotificationService.Models.Webhooks;
using NotificationService.Services;

namespace NotificationService.BLL.Tests.Services;

public class WebhooksProcessorTests
{
    private readonly Mock<IWebhooksConsumer> _consumerMock;
    private readonly Mock<IWebhookSubscriptionsStore> _subscriptionsStoreMock;
    private readonly Mock<IWebhookTriggeredProducer> _triggeredProducerMock;
    private readonly Mock<ILogger<WebhooksProcessor>> _loggerMock;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WebhooksProcessor _sut;
    private readonly Fixture _fixture = new();

    public WebhooksProcessorTests()
    {
        _consumerMock = new Mock<IWebhooksConsumer>();
        _subscriptionsStoreMock = new Mock<IWebhookSubscriptionsStore>();
        _triggeredProducerMock = new Mock<IWebhookTriggeredProducer>();
        _loggerMock = new Mock<ILogger<WebhooksProcessor>>();

        var services = new ServiceCollection();
        services.AddSingleton(_subscriptionsStoreMock.Object);
        services.AddSingleton(_triggeredProducerMock.Object);
        _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

        _sut = new WebhooksProcessor(_consumerMock.Object, _scopeFactory, _loggerMock.Object);
    }

    [Fact]
    public async Task StartAsync_OnStart_AddsSubscriptionToConsumer()
    {
        await _sut.StartAsync(CancellationToken.None);

        _consumerMock.Verify(x => x.AddSubscription(It.IsAny<Func<WebhooksQueueModel, Task>>()), Times.Once);
    }

    [Fact]
    public async Task StartAsync_ConsumerThrows_LogsAndRethrows()
    {
        var error = new InvalidOperationException("boom");
        _consumerMock.Setup(x => x.AddSubscription(It.IsAny<Func<WebhooksQueueModel, Task>>()))
            .Throws(error);

        Func<Task> act = () => _sut.StartAsync(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Process_MatchingSubscriptions_PublishesTriggeredEventForEach()
    {
        Func<WebhooksQueueModel, Task>? captured = null;
        _consumerMock.Setup(x => x.AddSubscription(It.IsAny<Func<WebhooksQueueModel, Task>>()))
            .Callback<Func<WebhooksQueueModel, Task>>(cb => captured = cb);

        var eventType = _fixture.Create<string>();
        var data = new { payload = "v" };
        var subscriptions = new List<WebhookSubscriptionModel>
        {
            new() { Id = "1", Url = "https://a", Action = eventType, CreationTime = DateTime.UtcNow },
            new() { Id = "2", Url = "https://b", Action = eventType, CreationTime = DateTime.UtcNow }
        };
        _subscriptionsStoreMock
            .Setup(x => x.GetManyAsync(It.Is<WebhooksFilter>(f => f.Actions != null && f.Actions.Contains(eventType)), OffsetPagination.None))
            .ReturnsAsync(new OffsetCollection<WebhookSubscriptionModel>(subscriptions, 0, subscriptions.Count));

        await _sut.StartAsync(CancellationToken.None);
        captured.Should().NotBeNull();
        await captured!(new WebhooksQueueModel { EventType = eventType, Data = data });

        foreach (var sub in subscriptions)
        {
            _triggeredProducerMock.Verify(x => x.Publish(It.Is<WebhookTriggeredQueueModel>(m =>
                m.Url == sub.Url
                && m.SubscriptionId == sub.Id
                && m.EventType == eventType
                && m.Data == data)), Times.Once);
        }
    }
}
