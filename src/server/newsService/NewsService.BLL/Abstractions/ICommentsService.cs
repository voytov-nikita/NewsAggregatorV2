using NewsService.Models.Comments;

namespace NewsService.BLL.Abstractions;

public interface ICommentsService
{
    public Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter);
    public Task UpdateAsync(CommentUpdateModel model);
    public Task CreateAsync(CommentCreateModel model);
    public Task RateAsync(int newsId, int commentId, string rateType);
    public Task DeleteAsync(int newsId, int commentId);
}