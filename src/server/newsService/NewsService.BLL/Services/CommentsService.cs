using NewsService.BLL.Abstractions;
using NewsService.Models.Comments;

namespace NewsService.BLL.Services;

public class CommentsService: ICommentsService
{
    public Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CommentUpdateModel model)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(CommentCreateModel model)
    {
        throw new NotImplementedException();
    }

    public Task RateAsync(int newsId, int commentId, string rateType)
    {
        throw new NotImplementedException();
    }
}