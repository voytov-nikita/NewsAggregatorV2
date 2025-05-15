using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace CrawlerService.API.Controllers;


[ApiController]
[Route("api/test")]
public class TestController: ControllerBase
{
    private IHttpClientFactory _httpClientFactory;

    public TestController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task Get()
    {
        HttpClient client = _httpClientFactory.CreateClient();
        
        HttpResponseMessage response = await client.GetAsync("https://www.pravda.com.ua/rss/");
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1254");
        
        //Encoding.RegisterProvider();
        
        var res = await response.Content.ReadAsByteArrayAsync();
              
        XmlDocument doc = new XmlDocument();
        
        var stream = new MemoryStream(res);
        
        doc.Load(stream);
        
    }

}