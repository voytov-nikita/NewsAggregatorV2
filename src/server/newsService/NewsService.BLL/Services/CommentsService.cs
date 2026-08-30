using Common.Auth.Constants;
using Common.Auth.Services;
using NewsService.BLL.Abstractions;
using NewsService.BLL.Abstractions.Services;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.Comments;
using NewsService.Models.Comments.Enums;
using NewsService.Models.Comments.Models;

namespace NewsService.BLL.Services;

public class CommentsService: ICommentsService
{
    private readonly ICommentsStore _commentsStore;
    private readonly ICurrentUser _currentUser;

    public CommentsService(ICommentsStore commentsStore, ICurrentUser currentUser)
    {
        _commentsStore = commentsStore;
        _currentUser = currentUser;
    }

    public async Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter, int newsId)
    {
        return await _commentsStore.GetManyAsync(filter, newsId);
    }

    public async Task UpdateAsync(CommentUpdateModel model)
    {
        await EnsureCanModifyAsync(model.NewsId, model.CommentId);

        await _commentsStore.UpdateAsync(model);
    }

    public async Task CreateAsync(CommentCreateModel model)
    {
        await _commentsStore.CreateAsync(model);
    }

    public async Task RateAsync(int newsId, int commentId, RateType rateType)
    {
       await _commentsStore.RateAsync(newsId, commentId, rateType);
    }
    
    public async Task DeleteAsync(int newsId, int commentId)
    {
        await EnsureCanModifyAsync(newsId, commentId);

        await _commentsStore.DeleteAsync(newsId, commentId);
    }

    #region private

    /// <summary>
    /// Editing and deleting are allowed to the author, or to anyone holding comment.moderate.
    /// The actor is read from ICurrentUser rather than taken from the model: an identity that
    /// travels in a payload can be forged by whoever builds that payload.
    /// </summary>
    private async Task EnsureCanModifyAsync(int newsId, int commentId)
    {
        if (_currentUser.HasPermission(Permissions.CommentModerate))
        {
            return;
        }

        Guid? authorId = await _commentsStore.GetAuthorIdAsync(newsId, commentId);

        if (authorId is null)
        {
            throw new KeyNotFoundException($"Comment {commentId} not found");
        }

        if (authorId != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException("Only the author can modify this comment.");
        }
    }

    #endregion
}
