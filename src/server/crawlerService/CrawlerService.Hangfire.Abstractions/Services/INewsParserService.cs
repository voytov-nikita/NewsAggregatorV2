namespace CrawlerService.Hangfire.Abstractions.Services;

public interface INewsParserService
{
    Task ParseAsync(byte[] data);
}