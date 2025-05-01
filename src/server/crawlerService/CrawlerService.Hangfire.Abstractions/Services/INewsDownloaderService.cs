namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsDownloaderService
{
    Task GetNewsAsync();
}