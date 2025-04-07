using NewsService.API.Models.News;
using NewsService.Models.News;

namespace NewsService.API.Extensions.News;

public static class NewsRequestExtensions
{
    public static NewsResponse ToResponse(this NewsModel model)
    {
        return new NewsResponse() { };
    }
    
    public static NewsFilterModel ToModel(this NewsFilterRequest filterRequest)
    {
        return new NewsFilterModel() { };
    }
}