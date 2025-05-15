using CrawlerService.BLL.Services;
using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Models.Models;
using Hangfire;

namespace CrawlerService.Hangfire.Services;

class NewsUniquenessService : INewsUniquenessService
{
    private IBackgroundJobClient _backgroundJobClient;
    private NewsService _newsService;

    public NewsUniquenessService(IBackgroundJobClient backgroundJobClient, NewsService newsService)
    {
        _backgroundJobClient = backgroundJobClient;
        _newsService = newsService;
    }

    public async Task IsUniqueBulkAsync(ParsedNews[] data)
    {
        Console.WriteLine("Checking on uniqueness");

        var parsedNewsGuides = data.Select(_ => _.CompositeGuid).ToArray();
        
        List<ParsedNews> lastReadNews = await _newsService.GetLastReadNewsAsync(parsedNewsGuides);

        //In case of absence last read news
        if (!lastReadNews.Any())
        {
            await _newsService.UpdateLastReadNewsAsync(data, "Pravda");
            _backgroundJobClient.Enqueue<INewsQueueService>(service => service.AddManyToQueueAsync(data));
        }
        else
        {
            IEnumerable<string> lastReadCustomGuides = lastReadNews.Select(_ => _.CompositeGuid);
            
            bool hasChanges = parsedNewsGuides.Except(lastReadCustomGuides).Any();

            if (hasChanges)
            {
                //Todo: move to const "Pravda"
                ParsedNews[] a = data.Where(_ => !lastReadCustomGuides.Contains(_.CompositeGuid)).ToArray();
                
                await _newsService.UpdateLastReadNewsAsync(data, "Pravda");
                _backgroundJobClient.Enqueue<INewsQueueService>(service => service.AddManyToQueueAsync(a));
            }
        }
        
        Console.WriteLine("Checking complete");
        
    }
}