using Common.API.Models;
using NewsService.Models.Comments.Enums;

namespace NewsService.API.Models.Comments;

public class CommentsFilterRequest: BaseFilterRequest<CommentsOrderField>
{
    public int Take { get; set; }
    public int Offset { get; set; }
}