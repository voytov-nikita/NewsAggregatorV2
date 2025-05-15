using CrawlerService.DAL.Stores;
using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Services;

public class NewsService
{
    private readonly NewsStore _newsStore;

    public NewsService(NewsStore newsStore)
    {
        _newsStore = newsStore;
    }

    public async Task UpdateLastReadNewsAsync(ParsedNews[] parsedNews, string publisherName)
    {
        //Add validation, to prevent wrong publisherName insert
        
        await _newsStore.UpdateLastReadNewsAsync(parsedNews, publisherName);

    }
    public async Task<List<ParsedNews>> GetLastReadNewsAsync(string[] uniqueStrings)
    {
        return await _newsStore.GetLastReadNewsAsync(uniqueStrings);

    }
}