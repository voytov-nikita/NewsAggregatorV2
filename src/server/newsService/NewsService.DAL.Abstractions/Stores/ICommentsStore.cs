using NewsService.Models.Comments;
using NewsService.Models.Comments.Enums;
using NewsService.Models.Comments.Models;

namespace NewsService.DAL.Abstractions.Stores;

public interface ICommentsStore
{
    public Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter, int newsId);
    public Task UpdateAsync(CommentUpdateModel model);
    public Task CreateAsync(CommentCreateModel model);
    public Task RateAsync(int newsId, int commentId, RateType rateType);
    public Task DeleteAsync(int newsId, int commentId);

    /// <summary>Returns the comment's author, or null when the comment does not exist.</summary>
    public Task<Guid?> GetAuthorIdAsync(int newsId, int commentId);
}
