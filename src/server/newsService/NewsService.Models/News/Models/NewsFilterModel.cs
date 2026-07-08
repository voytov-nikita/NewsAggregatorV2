using Common.Models;
using NewsService.Models.News.Enum;
// NewsOrderField from NewsService.Models.News.Enum; NewsCategory from Common.Models.

namespace NewsService.Models.News.Models;

public class NewsFilterModel : BaseFilter<NewsOrderField>
{
    public int Take { get; set; }
    public int Offset { get; set; }
    public string? Keyword { get; set; }

    public NewsCategory[]? Categories { get; set; }
    public string[]? Sources { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
