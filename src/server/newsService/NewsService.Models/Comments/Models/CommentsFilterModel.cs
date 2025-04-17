using Common.Models;
using NewsService.Models.Comments.Enums;

namespace NewsService.Models.Comments.Models;

public class CommentsFilterModel: BaseFilter<CommentsOrderField>
{
    public int Take { get; set; }
    public int Offset { get; set; }
}