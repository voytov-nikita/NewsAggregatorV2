using NewsService.DAL.Abstractions;
using NewsService.Models.News;

namespace NewsService.DAL.Stores;

public class NewsStore: INewsStore
{
    public async Task<NewsModel[]> GetManyAsync(NewsFilterModel filter)
    {
        return [new NewsModel
            {
                Id = 0,
                Title = "Ttile",
                Description = "Description",
                OriginalLink = "OriginalLink",
                PublishDate = DateTime.UtcNow,
                ReadDate = DateTime.UtcNow,
                ImageLink = "ImageLink",
                Publisher = "Publisher",
                PublisherLink = "PublisherLink",
            }
        ];
    }

    public async Task CreateAsync(NewsCreateModel model)
    {
    }
}