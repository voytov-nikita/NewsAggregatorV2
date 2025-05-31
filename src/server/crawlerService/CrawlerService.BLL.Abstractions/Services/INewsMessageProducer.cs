using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Abstractions.Services;

public interface INewsMessageProducer
{
    Task Send(ParsedNews message);
}