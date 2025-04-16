using NewsService.BLL.Abstractions;
using NewsService.BLL.Abstractions.Services;
using NewsService.DAL.Abstractions.Stores;
using NewsService.Models.Comments;
using NewsService.Models.Enums;

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

    public async Task RateAsync(int newsId, int commentId, RateType rateType)
    {
       await _commentsStore.RateAsync(newsId, commentId, rateType);
    }
    
    public async Task DeleteAsync(int newsId, int commentId)
    {
       await _commentsStore.DeleteAsync(newsId, commentId);
    }
}