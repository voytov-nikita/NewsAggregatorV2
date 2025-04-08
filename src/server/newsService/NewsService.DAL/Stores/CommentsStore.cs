using NewsService.DAL.Abstractions;
using NewsService.Models.Comments;

namespace NewsService.DAL.Stores;

public class CommentsStore: ICommentsStore
{
    public async Task<CommentsModel[]> GetManyAsync(CommentsFilterModel filter)
    {
        return [new CommentsModel
            {
                Id = 0,
                Content = "Ttile",
                CreatorName = "Description",
                CreatorGuid = "OriginalLink",
                CreateDate = DateTime.UtcNow,
            }
        ];
    }

    public async Task UpdateAsync(CommentUpdateModel model)
    {
    }

    public async Task CreateAsync(CommentCreateModel model)
    {
    }

    public async Task RateAsync(int newsId, int commentId, string rateType)
    {
    }
}