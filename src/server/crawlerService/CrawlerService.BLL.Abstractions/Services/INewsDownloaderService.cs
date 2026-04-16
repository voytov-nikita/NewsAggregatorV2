namespace CrawlerService.BLL.Abstractions.Services;

public interface INewsDownloaderService
{
    Task GetNewsAsync(string sourceId);
}
