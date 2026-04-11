using System.Xml;
using Common.Const;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.Models.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services;

class NewsParserService : INewsParserService
{
    private readonly IPostponedJobRunner _postponedJobRunner;
    private readonly ILogger<NewsParserService> _logger;

    public NewsParserService(IPostponedJobRunner postponedJobRunner, ILogger<NewsParserService> logger)
    {
        _postponedJobRunner = postponedJobRunner;
        _logger = logger;
    }

    public async Task ParseAsync(byte[] data)
    {
        _logger.LogInformation("Parsing started");

        XmlDocument doc = new XmlDocument();

        var stream = new MemoryStream(data);
        try
        {
            doc.Load(stream);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse RSS feed");
            return;
        }

        List<ParsedNews> parsedNews = [];

        XmlNodeList elementsByTagName = doc.GetElementsByTagName("item");

        if (elementsByTagName.Count == 0)
        {
            _logger.LogWarning("No news items found in the feed");
            return;
        }

        foreach (XmlNode item in elementsByTagName)
        {
            ParsedNews parsedNew = ToParsedNew(item);

            parsedNews.Add(parsedNew);
        }

        ParsedNews[] newsArray = parsedNews.ToArray();

        _postponedJobRunner.Enqueue<INewsService>(service => service.SaveUniqueNewsAsync(newsArray));

        _logger.LogInformation("Parsing complete: {Count} items", newsArray.Length);
    }

    private static ParsedNews ToParsedNew(XmlNode item)
    {
        return new ParsedNews
        {
            Title = item["title"].InnerText,
            Description = item["description"]?.InnerText,
            OriginalLink = item["link"].InnerText,
            ImageLink = item["enclosure"]?.GetAttribute("url"),
            PublishDate = DateTime.Parse(item["pubDate"].InnerText),
            PublisherName = Publishers.Pravda,
            //Todo: workaround, investigate about source scaling
            PublisherLink = "https://www.pravda.com.ua/",
            PublisherGuid = item["guid"]?.InnerText,
            //Create custom guid to check uniqueness
            Guid = item["guid"]?.InnerText,
            GlobalUniqueId = Publishers.Pravda + item["guid"]?.InnerText
        };
    }
}
