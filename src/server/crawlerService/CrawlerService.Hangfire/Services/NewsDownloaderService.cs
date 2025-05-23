using System.Text;
using CrawlerService.Hangfire.Abstractions.Services;
using Hangfire;

namespace CrawlerService.Hangfire.Services;

public class NewsDownloaderService: INewsDownloaderService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public NewsDownloaderService(IBackgroundJobClient backgroundJobClient, IHttpClientFactory httpClientFactory)
    {
        _backgroundJobClient = backgroundJobClient;
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

        _backgroundJobClient.Enqueue<INewsParserService>(service => service.ParseAsync(res));
        
        
        Console.WriteLine("Crawling complete");
        Console.WriteLine("``````````````````````");
    }
    
}