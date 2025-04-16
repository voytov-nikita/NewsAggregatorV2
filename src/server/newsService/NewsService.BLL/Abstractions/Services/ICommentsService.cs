using NewsService.Models.Comments;
using NewsService.Models.Enums;

namespace NewsService.BLL.Abstractions.Services;

public interface ICommentsService
{
    public Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter);
    public Task UpdateAsync(CommentUpdateModel model);
    public Task CreateAsync(CommentCreateModel model);
    public Task RateAsync(int newsId, int commentId, RateType rateType);
    public Task DeleteAsync(int newsId, int commentId);
}