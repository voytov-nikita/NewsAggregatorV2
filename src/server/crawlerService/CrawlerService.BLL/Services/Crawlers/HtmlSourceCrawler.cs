using AngleSharp;
using AngleSharp.Dom;
using CrawlerService.BLL.Abstractions.Services;
using CrawlerService.BLL.Services.Mapping;
using CrawlerService.Models.Enums;
using CrawlerService.Models.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services.Crawlers;

internal class HtmlSourceCrawler : ISourceCrawler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HtmlSourceCrawler> _logger;

    public HtmlSourceCrawler(IHttpClientFactory httpClientFactory, ILogger<HtmlSourceCrawler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public SourceType Type => SourceType.Html;

    public async Task<ParsedNews[]> CrawlAsync(NewsSource source)
    {
        if (source.HtmlSelectors is null)
        {
            _logger.LogError("HTML source {SourceId} is missing selectors configuration", source.Id);
            return Array.Empty<ParsedNews>();
        }

        HtmlSelectors selectors = source.HtmlSelectors;

        _logger.LogInformation("Crawling HTML {SourceId} ({Url})", source.Id, source.Url);

        string html;
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            html = await client.GetStringAsync(source.Url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download HTML {SourceId}", source.Id);
            return Array.Empty<ParsedNews>();
        }

        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(req => req.Content(html));

        IHtmlCollection<IElement> items = document.QuerySelectorAll(selectors.ItemSelector);
        if (items.Length == 0)
        {
            _logger.LogWarning("HTML {SourceId}: no items matched selector '{Selector}'", source.Id, selectors.ItemSelector);
            return Array.Empty<ParsedNews>();
        }

        List<ParsedNews> result = new(items.Length);
        foreach (IElement item in items)
        {
            string title = HtmlFieldExtractor.ExtractSingle(item, selectors.Title) ?? string.Empty;
            string link = HtmlFieldExtractor.ExtractSingle(item, selectors.Link) ?? string.Empty;
            string? description = HtmlFieldExtractor.ExtractSingle(item, selectors.Description);
            string? image = HtmlFieldExtractor.ExtractSingle(item, selectors.Image);
            string? publishRaw = HtmlFieldExtractor.ExtractSingle(item, selectors.PublishDate);
            string? guidOverride = HtmlFieldExtractor.ExtractSingle(item, selectors.Guid);

            string guid = !string.IsNullOrEmpty(guidOverride) ? guidOverride : link;

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

        _logger.LogInformation("HTML {SourceId}: parsed {Count} items", source.Id, result.Count);
        return result.ToArray();
    }
}
