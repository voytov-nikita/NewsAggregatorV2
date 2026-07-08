using NewsService.API.Models.News;
using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.API.Extensions.News;

public static class NewsRequestExtensions
{
    public static NewsResponse ToResponse(this NewsModel model)
    {
        return new NewsResponse
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            OriginalLink = model.OriginalLink,
            PublishDate = model.PublishDate,
            ReadDate = model.ReadDate,
            ImageLink = model.ImageLink,
            Publisher = model.Publisher,
            PublisherLink = model.PublisherLink,
            Category = model.Category,
            ReadTimeMinutes = model.ReadTimeMinutes,
            Likes = model.Likes,
            Dislikes = model.Dislikes,
            CommentsCount = model.CommentsCount,
        };
    }

    public static NewsFilterModel ToModel(this NewsFilterRequest filterRequest)
    {
        return new NewsFilterModel
        {
            Take = filterRequest.Take,
            Offset = filterRequest.Offset,
            OrderBy = filterRequest.OrderBy,
            OrderDirection = filterRequest.OrderDirection,
            Keyword = filterRequest.Keyword,
            Categories = filterRequest.Categories,
            Sources = filterRequest.Sources,
            DateFrom = filterRequest.DateFrom,
            DateTo = filterRequest.DateTo,
        };
    }
}
