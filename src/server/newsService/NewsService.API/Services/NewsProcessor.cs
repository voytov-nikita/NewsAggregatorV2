using MessageQueue.Abstractions;
using MessageQueue.Models;
using NewsService.BLL.Abstractions.Services;
using NewsService.Models.News.Models;

namespace NewsService.API.Services;

public class NewsProcessor : BackgroundService
{
    private readonly INewsConsumer _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public NewsProcessor(INewsConsumer consumer, IServiceScopeFactory serviceScopeFactory)
    {
        _consumer = consumer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.AddSubscription(Process);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task Process(NewsQueueModel[] messageModel)
    {
        //Todo: Console.WriteLine replace with Logger
        Console.WriteLine("Processing message...");
        if (messageModel.Length != 0)
        {

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
            
            //Workaround
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            INewsService newsService = scope.ServiceProvider.GetRequiredService<INewsService>();
            
            await newsService.CreateBulkAsync(createModels);

            IWebhookDispatcher webhookDispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcher>();
            webhookDispatcher.Dispatch("news.created", createModels);

            Console.WriteLine("------------=======-------------");
            Console.WriteLine($"{messageModel.Length} new News were added ({DateTime.Now})");
            Console.WriteLine("------------=======-------------");
        }
        else
        {
            Console.WriteLine("------------=======-------------");
            Console.WriteLine("No new news were added");
            Console.WriteLine("------------=======-------------");
        }
        Console.WriteLine("****************************");
    }
}