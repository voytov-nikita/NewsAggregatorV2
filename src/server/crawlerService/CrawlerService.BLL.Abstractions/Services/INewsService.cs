using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Abstractions.Services;

public interface INewsService
{
    Task SaveUniqueNewsAsync(ParsedNews[] parsedNews);
}