using System.Xml;
using CrawlerService.Hangfire.Abstractions.Services;
using CrawlerService.Models.Models;
using Hangfire;

namespace CrawlerService.Hangfire.Services;

class NewsParserService : INewsParserService
{
    private const string PublisherName = "Pravda";
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NewsParserService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }
    
    public async Task ParseAsync(byte[] data)
    {
        Console.WriteLine("Parsing...");
        
        XmlDocument doc = new XmlDocument();
        
        var stream = new MemoryStream(data);

        try
        {
            doc.Load(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        List<ParsedNews> parsedNews = [];

        foreach (XmlNode item in doc.GetElementsByTagName("item"))
        {
            var parsedNew = new ParsedNews
            {
                Title = item["title"].InnerText,
                Description = item["description"]?.InnerText,
                OriginalLink = item["link"].InnerText,
                ImageLink = item["enclosure"]?.GetAttribute("url"),
                PublishDate = DateTime.Parse(item["pubDate"].InnerText),
                PublisherName = PublisherName,
                //Todo: workaround, investigate about source scaling
                PublisherLink = "https://www.pravda.com.ua/",
                PublisherGuid = item["guid"]?.InnerText,
                //Create custom guid to check uniqueness
                CompositeGuid = PublisherName + item["guid"]?.InnerText
            };
            parsedNews.Add(parsedNew);
        }
        
        Console.WriteLine("Parsing complete");
        
        _backgroundJobClient.Enqueue<INewsUniquenessService>(service => service.IsUniqueBulkAsync(parsedNews.ToArray()));
    }
}