using Moq;

namespace CrawlerService.BLL.Tests.Helpers;

internal class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_responder(request));
    }
}

internal static class HttpClientFactoryStub
{
    public static IHttpClientFactory ReturningBytes(byte[] content)
    {
        StubHttpMessageHandler handler = new(_ => new HttpResponseMessage
        {
            Content = new ByteArrayContent(content),
        });
        return ForHandler(handler);
    }

    public static IHttpClientFactory ReturningString(string content)
    {
        StubHttpMessageHandler handler = new(_ => new HttpResponseMessage
        {
            Content = new StringContent(content),
        });
        return ForHandler(handler);
    }

    public static IHttpClientFactory Throwing(Exception exception)
    {
        StubHttpMessageHandler handler = new(_ => throw exception);
        return ForHandler(handler);
    }

    private static IHttpClientFactory ForHandler(HttpMessageHandler handler)
    {
        HttpClient client = new(handler);
        Mock<IHttpClientFactory> factory = new();
        factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
        return factory.Object;
    }
}
