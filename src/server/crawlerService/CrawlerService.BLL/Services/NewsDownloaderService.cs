using CrawlerService.BLL.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace CrawlerService.BLL.Services;

public class NewsDownloaderService: INewsDownloaderService
{
    private readonly IPostponedJobRunner _postponedJobRunner;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NewsDownloaderService> _logger;

    public NewsDownloaderService(
        IPostponedJobRunner postponedJobRunner,
        IHttpClientFactory httpClientFactory,
        ILogger<NewsDownloaderService> logger)
    {
        _postponedJobRunner = postponedJobRunner;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task GetNewsAsync()
    {
        _logger.LogInformation("Crawling started");

        HttpClient client = _httpClientFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("https://www.pravda.com.ua/rss/");

        byte[] res = await response.Content.ReadAsByteArrayAsync();

        _postponedJobRunner.Enqueue<INewsParserService>(service => service.ParseAsync(res));

        _logger.LogInformation("Crawling complete");
    }
}
