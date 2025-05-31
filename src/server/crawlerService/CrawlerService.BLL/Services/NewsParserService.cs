using System.Xml;
using Common.Const;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.Models.Models;

namespace CrawlerService.BLL.Services;

class NewsParserService : INewsParserService
{
    private readonly IPostponedJobRunner _postponedJobRunner;

    public NewsParserService(IPostponedJobRunner postponedJobRunner)
    {
        _postponedJobRunner = postponedJobRunner;
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
            Console.WriteLine("Parsing Error:");
            Console.WriteLine(e.Message);
        }

        List<ParsedNews> parsedNews = [];
        
        foreach (XmlNode item in doc.GetElementsByTagName("item"))
        {
            ParsedNews parsedNew = ToParsedNew(item);

            parsedNews.Add(parsedNew);
        }

        ParsedNews[] newsArray = parsedNews.ToArray();

        _postponedJobRunner.Enqueue<INewsService>(service => service.SaveUniqueNewsAsync(newsArray));

        Console.WriteLine("Parsing complete");
        Console.WriteLine("``````````````````````");
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