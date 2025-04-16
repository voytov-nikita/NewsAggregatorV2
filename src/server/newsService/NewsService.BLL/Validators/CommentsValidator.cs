using NewsService.BLL.Abstractions.Validators;
using NewsService.Models.Comments;

namespace NewsService.BLL.Validators;

public class CommentsValidator: ICommentsValidator
{
    private readonly List<string> rateOptions = ["Like", "Dislike"];
    
    public void Validate(CommentsFilterModel filter)
    {
        //TODO: Investigate about 
    }

    public void Validate(CommentUpdateModel model)
    {
        if (string.IsNullOrEmpty(model.Content))
        {
            throw new ArgumentException("Content is empty");
        }
        
        //TODO: Investigate about comment length, add to CommentsEntityConfiguration
        if (model.Content.Length > 1000)
        {
            throw new ArgumentException("Content is empty");
        }
    }

    public void Validate(CommentCreateModel model)
    {
        
        if (string.IsNullOrEmpty(model.Content))
        {
            throw new ArgumentException("Content is empty");
        }
        
        //TODO: Investigate about comment length, add to CommentsEntityConfiguration
        if (model.Content.Length > 1000)
        {
            throw new ArgumentException("Content is empty");
        }
    }

    public void Validate(string rateType)
    {
        if (string.IsNullOrEmpty(rateType))
        {
            throw new ArgumentException("RateType is empty");
        }
        
        if (!rateOptions.Contains(rateType))
        {
            throw new ArgumentException("RateType is not valid");
        }
    }
}