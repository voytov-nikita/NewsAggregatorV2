using Common.API.Models;
using Common.Models;
using NewsService.Models.News.Enum;

namespace NewsService.API.Models.News;

public class NewsFilterRequest: BaseFilterRequest<NewsOrderField>
{
    public int Take { get; set; }
    public int Offset { get; set; }
    public string? Keyword { get; set; }

    public NewsCategory[]? Categories { get; set; }
    public string[]? Sources { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
