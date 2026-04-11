using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.DAL.Abstractions.Stores;
using CrawlerService.Models.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services;

internal class NewsService : INewsService
{
    private readonly INewsStore _newsStore;
    private readonly IPostponedJobRunner _postponedJobRunner;
    private readonly ILogger<NewsService> _logger;

    public NewsService(INewsStore newsStore, IPostponedJobRunner postponedJobRunner, ILogger<NewsService> logger)
    {
        _newsStore = newsStore;
        _postponedJobRunner = postponedJobRunner;
        _logger = logger;
    }

    public async Task SaveUniqueNewsAsync(ParsedNews[] parsedNews)
    {
        //Todo: Discuss with SergeyV
        parsedNews = Unify(parsedNews);
        
        Dictionary<string, ParsedNews> dictionary = parsedNews.ToDictionary(_ => _.GlobalUniqueId);

        string[] globalUniqueIds = dictionary.Keys.ToArray();

        string[] notExistedIds = await _newsStore.ExcludeExistedAsync(globalUniqueIds);

        ParsedNews[] notExistsNews = dictionary.Where(_ => notExistedIds.Contains(_.Key))
            .Select(_ => _.Value)
            .ToArray();

        if (notExistedIds.Any())
        {
            await _newsStore.InsertAsync(notExistsNews);
            
            _postponedJobRunner.Enqueue<INewsQueueService>(_ => _.AddManyToQueueAsync(notExistsNews));
        }
        else
        {
            _logger.LogInformation("All news already exists in the database. No new news to save");
        }
        
    }
    
    private ParsedNews[] Unify(ParsedNews[] parsedNews)
    {
        // Some ParsedNews have the same CompositeGuid, but have different PublishDate. Return unique ParsedNews. If there are multiple ParsedNews with the same CompositeGuid, keep the one with the latest PublishDate.
        // *** Made by AI ***
        return parsedNews
            .GroupBy(_ => _.GlobalUniqueId)
            .Select(_ => _.OrderByDescending(x => x.PublishDate).First())
            .ToArray();
    }
}