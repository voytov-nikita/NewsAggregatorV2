using Common.Models;
using NewsService.Models.News.Enum;

namespace NewsService.Models.News.Models;

public class NewsFilterModel : BaseFilter<NewsOrderField>
{
    public int Take { get; set; }
    public int Offset { get; set; }
    public string? Keyword { get; set; }
}