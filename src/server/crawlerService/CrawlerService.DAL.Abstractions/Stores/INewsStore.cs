using CrawlerService.Models.Models;

namespace CrawlerService.DAL.Abstractions.Stores;

public interface INewsStore
{
    Task InsertAsync(ParsedNews[] parsedNews);

    Task<string[]> ExcludeExistedAsync(string[] globalUniqueIds);
}