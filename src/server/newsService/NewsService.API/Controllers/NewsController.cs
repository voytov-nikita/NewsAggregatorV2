using Microsoft.AspNetCore.Mvc;
using NewsService.API.Extensions.News;
using NewsService.API.Models.News;
using NewsService.BLL.Abstractions;
using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.API.Controllers;

[ApiController]
[Route("api/v1/news")]
public class NewsController: ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet("")]
    public async Task<List<NewsResponse>> GetMany([FromQuery] NewsFilterRequest filterRequest)
    {
        NewsFilterModel filter = filterRequest.ToModel();
        NewsModel[] result = await _newsService.GetManyAsync(filter);

        //AddHeaders
        
        return result.Select(_ => _.ToResponse()).ToList();
    }

}