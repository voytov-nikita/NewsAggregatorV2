using NewsService.Models.Comments;

namespace NewsService.BLL.Abstractions.Validators;

public interface ICommentsValidator
{
    public void Validate(CommentsFilterModel filter);
    public void Validate(CommentUpdateModel model);
    public void Validate(CommentCreateModel model);
    public void Validate(string rateType);
}