using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NotificationService.BLL.Abstractions.Services;
using NotificationService.Models.Webhooks;
using NotificationService.Services;

namespace NotificationService.BLL.Tests.Services;

public class WebhookDeliveryServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IWebhookDeliveryAttemptService> _deliveryAttemptServiceMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly WebhookDeliveryService _sut;
    private readonly Fixture _fixture = new();

    public WebhookDeliveryServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _deliveryAttemptServiceMock = new Mock<IWebhookDeliveryAttemptService>();

        _sut = new WebhookDeliveryService(_httpClientFactoryMock.Object, _deliveryAttemptServiceMock.Object);
    }

    [Fact]
    public async Task DeliverAsync_ValidModel_PostsPayloadAndRecordsAttempt()
    {
        var model = new WebhookDispatchModel
        {
            Url = "https://example.test/webhook",
            SubscriptionId = _fixture.Create<string>(),
            EventType = _fixture.Create<string>(),
            Payload = new { foo = "bar" }
        };
        const HttpStatusCode statusCode = HttpStatusCode.OK;
        const string responseBody = "ok";

        HttpRequestMessage? capturedRequest = null;
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody)
            });

        await _sut.DeliverAsync(model);

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Post);
        capturedRequest.RequestUri!.ToString().Should().Be(model.Url);
        var sentPayload = await capturedRequest.Content!.ReadFromJsonAsync<WebhookPayload>();
        sentPayload!.EventType.Should().Be(model.EventType);
        sentPayload.SubscriptionId.Should().Be(model.SubscriptionId);

        _deliveryAttemptServiceMock.Verify(x => x.AddAsync(It.Is<WebhookDeliveryAttemptModel>(m =>
            m.SubscriptionId == model.SubscriptionId
            && m.ResponseStatusCode == (int)statusCode
            && m.ResponseMessage == responseBody)), Times.Once);
    }

    [Fact]
    public async Task DeliverManyAsync_MultipleModels_DeliversEach()
    {
        var models = Enumerable.Range(0, 3).Select(i => new WebhookDispatchModel
        {
            Url = $"https://example.test/{i}",
            SubscriptionId = _fixture.Create<string>(),
            EventType = _fixture.Create<string>(),
            Payload = null
        }).ToArray();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty)
            });

        await _sut.DeliverManyAsync(models);

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(models.Length),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        _deliveryAttemptServiceMock.Verify(
            x => x.AddAsync(It.IsAny<WebhookDeliveryAttemptModel>()),
            Times.Exactly(models.Length));
    }
}
