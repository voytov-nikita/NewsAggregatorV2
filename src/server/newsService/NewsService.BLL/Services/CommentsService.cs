using NewsService.BLL.Abstractions;
using NewsService.DAL.Abstractions;
using NewsService.Models.Comments;

namespace NewsService.BLL.Services;

public class CommentsService: ICommentsService
{
    private readonly ICommentsStore _commentsStore;

    public CommentsService(ICommentsStore commentsStore)
    {
        _commentsStore = commentsStore;
    }

    public async Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter)
    {
        return await _commentsStore.GetManyAsync(filter);
    }

    public async Task UpdateAsync(CommentUpdateModel model)
    {
        await _commentsStore.UpdateAsync(model);
    }

    public async Task CreateAsync(CommentCreateModel model)
    {
        await _commentsStore.CreateAsync(model);
    }

    public async Task RateAsync(int newsId, int commentId, string rateType)
    {
       await _commentsStore.RateAsync(newsId, commentId, rateType);
    }
    
    public async Task DeleteAsync(int newsId, int commentId)
    {
       await _commentsStore.DeleteAsync(newsId, commentId);
    }
}