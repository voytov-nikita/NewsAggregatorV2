using System.Text;
using Microsoft.Extensions.Http;
using CrawlerService.BLL.Abstractions.Services;

namespace CrawlerService.BLL.Services;

public class NewsDownloaderService: INewsDownloaderService
{
    private readonly IPostponedJobRunner _postponedJobRunner;
    private readonly IHttpClientFactory _httpClientFactory;

    public NewsDownloaderService(IPostponedJobRunner postponedJobRunner, IHttpClientFactory httpClientFactory)
    {
        _postponedJobRunner = postponedJobRunner;
        _httpClientFactory = httpClientFactory;
    }

    public async Task GetNewsAsync()
    {
        //replace with Logger
        Console.WriteLine("**********************");
        Console.WriteLine($"Crawling...  ({DateTime.Now})");


        HttpClient client = _httpClientFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("https://www.pravda.com.ua/rss/");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1254");

        byte[] res = await response.Content.ReadAsByteArrayAsync();

        _postponedJobRunner.Enqueue<INewsParserService>(service => service.ParseAsync(res));


        Console.WriteLine("Crawling complete");
        Console.WriteLine("``````````````````````");
    }

}