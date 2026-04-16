using MessageQueue.Abstractions;
using MessageQueue.Models;
using NewsService.BLL.Abstractions.Services;
using NewsService.Models.News.Models;

namespace NewsService.API.Services;

public class NewsProcessor : BackgroundService
{
    private readonly INewsConsumer _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NewsProcessor> _logger;

    public NewsProcessor(
        INewsConsumer consumer,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NewsProcessor> logger)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.AddSubscription(Process);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to subscribe to news queue");
            throw;
        }

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task Process(NewsQueueModel[] messageModel)
    {
        if (messageModel.Length == 0)
        {
            _logger.LogInformation("Received empty news batch, nothing to process");
            return;
        }

        NewsCreateModel[] createModels = messageModel.Select(_ => new NewsCreateModel
        {
            Title = _.Title,
            Description = _.Description,
            OriginalLink = _.OriginalLink,
            ImageLink = _.ImageLink,
            PublishDate = _.PublishDate,
            Publisher = _.PublisherName,
            PublisherLink = _.PublisherLink,
            Guid = _.Guid,
        }).ToArray();

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        INewsService newsService = scope.ServiceProvider.GetRequiredService<INewsService>();

        await newsService.CreateBulkAsync(createModels);

        IWebhookDispatcher webhookDispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcher>();
        webhookDispatcher.Dispatch("news.created", createModels);

        _logger.LogInformation("Saved {Count} news items and dispatched webhook", createModels.Length);
    }
}
