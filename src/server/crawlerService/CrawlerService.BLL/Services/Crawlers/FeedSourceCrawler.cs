using System.ServiceModel.Syndication;
using System.Xml;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services.Mapping;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services.Crawlers;

internal class FeedSourceCrawler : ISourceCrawler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FeedSourceCrawler> _logger;

    public FeedSourceCrawler(IHttpClientFactory httpClientFactory, ILogger<FeedSourceCrawler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public SourceType Type => SourceType.Feed;

    public async Task<ParsedNews[]> CrawlAsync(NewsSource source)
    {
        _logger.LogInformation("Crawling feed {SourceId} ({Url})", source.Id, source.Url);

        byte[] bytes;
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            bytes = await client.GetByteArrayAsync(source.Url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download feed {SourceId}", source.Id);
            return Array.Empty<ParsedNews>();
        }

        ParsedNews[] result;
        try
        {
            result = source.FeedMapping is null
                ? ParseWithSyndication(source, bytes)
                : ParseWithXPath(source, bytes, source.FeedMapping);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse feed {SourceId}", source.Id);
            return Array.Empty<ParsedNews>();
        }

        _logger.LogInformation("Feed {SourceId}: parsed {Count} items", source.Id, result.Length);
        return result;
    }

    private static ParsedNews[] ParseWithSyndication(NewsSource source, byte[] bytes)
    {
        using MemoryStream stream = new(bytes);
        using XmlReader reader = XmlReader.Create(stream);
        SyndicationFeed feed = SyndicationFeed.Load(reader);

        return feed.Items.Select(item => ToParsedNews(source, item)).ToArray();
    }

    private static ParsedNews ToParsedNews(NewsSource source, SyndicationItem item)
    {
        string guid = item.Id ?? item.Links.FirstOrDefault()?.Uri?.ToString() ?? string.Empty;
        string link = item.Links.FirstOrDefault(l => string.IsNullOrEmpty(l.RelationshipType) || l.RelationshipType == "alternate")
            ?.Uri?.ToString() ?? item.Links.FirstOrDefault()?.Uri?.ToString() ?? string.Empty;
        string? image = item.Links.FirstOrDefault(l => l.RelationshipType == "enclosure")?.Uri?.ToString();

        return new ParsedNews
        {
            Title = item.Title?.Text ?? string.Empty,
            Description = item.Summary?.Text,
            OriginalLink = link,
            ImageLink = image,
            PublishDate = item.PublishDate.UtcDateTime,
            SourceId = source.Id,
            PublisherName = source.PublisherName,
            PublisherLink = source.PublisherLink,
            PublisherGuid = guid,
            Guid = guid,
            GlobalUniqueId = source.Id + guid,
            Category = source.Category,
        };
    }

    private static ParsedNews[] ParseWithXPath(NewsSource source, byte[] bytes, FeedFieldMapping mapping)
    {
        XmlDocument doc = new();
        using (MemoryStream stream = new(bytes))
        {
            doc.Load(stream);
        }

        string itemXPath = mapping.ItemXPath ?? "//item";
        XmlNodeList? items = doc.SelectNodes(itemXPath);
        if (items is null || items.Count == 0)
        {
            return Array.Empty<ParsedNews>();
        }

        List<ParsedNews> result = new(items.Count);
        foreach (XmlNode item in items)
        {
            string guid = XmlFieldExtractor.ExtractSingle(item, mapping.Guid) ?? string.Empty;
            string link = XmlFieldExtractor.ExtractSingle(item, mapping.Link) ?? string.Empty;
            string title = XmlFieldExtractor.ExtractSingle(item, mapping.Title) ?? string.Empty;
            string? description = XmlFieldExtractor.ExtractSingle(item, mapping.Description);
            string? image = XmlFieldExtractor.ExtractSingle(item, mapping.Image);
            string? publishRaw = XmlFieldExtractor.ExtractSingle(item, mapping.PublishDate);

            DateTime publishDate = DateTime.TryParse(publishRaw, out DateTime parsed)
                ? parsed.ToUniversalTime()
                : DateTime.UtcNow;

            result.Add(new ParsedNews
            {
                Title = title,
                Description = description,
                OriginalLink = link,
                ImageLink = image,
                PublishDate = publishDate,
                SourceId = source.Id,
                PublisherName = source.PublisherName,
                PublisherLink = source.PublisherLink,
                PublisherGuid = guid,
                Guid = guid,
                GlobalUniqueId = source.Id + guid,
                Category = source.Category,
            });
        }

        return result.ToArray();
    }
}
