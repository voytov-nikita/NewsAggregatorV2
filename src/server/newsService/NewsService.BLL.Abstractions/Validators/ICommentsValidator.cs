using NewsService.Models.Comments;
using NewsService.Models.Comments.Models;

namespace NewsService.BLL.Abstractions.Validators;

public interface ICommentsValidator
{
    public void Validate(CommentsFilterModel filter);
    public void Validate(CommentUpdateModel model);
    public void Validate(CommentCreateModel model);
    public void Validate(string rateType);
}