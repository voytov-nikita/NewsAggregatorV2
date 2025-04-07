using Microsoft.AspNetCore.Mvc;
using NewsService.API.Extensions.News;
using NewsService.API.Models.News;
using NewsService.Models.News;

namespace NewsService.API.Controllers;

[ApiController]
[Route("api/v1/news")]
public class NewsController: ControllerBase
{
    [HttpGet]
    public async Task<List<NewsResponse>> GetMany(NewsFilterRequest filterRequest)
    {
        NewsFilterModel filter = filterRequest.ToModel();
        throw new NotImplementedException();
        NewsModel[] result;

        return result.Select(_ => _.ToResponse()).ToList();
    }

}