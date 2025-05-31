namespace CrawlerService.BLL.Abstractions.Services;

public interface INewsParserService
{
    Task ParseAsync(byte[] data);
}