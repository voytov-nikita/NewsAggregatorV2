using NewsService.Models.Comments;

namespace NewsService.DAL.Abstractions;

public interface ICommentsStore
{
    public Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter);
    public Task UpdateAsync(CommentUpdateModel model);
    public Task CreateAsync(CommentCreateModel model);
    public Task RateAsync(int newsId, int commentId, string rateType);
}