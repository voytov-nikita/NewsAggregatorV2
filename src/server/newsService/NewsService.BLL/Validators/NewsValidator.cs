using NewsService.BLL.Abstractions.Validators;
using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Validators;

public class NewsValidator: INewsValidator
{
    public void Validate(NewsFilterModel filter)
    {
        throw new NotImplementedException();
    }

    public void Validate(NewsCreateModel model)
    {
        if (string.IsNullOrEmpty(model.Title))
        {
            throw new ArgumentException("Title cannot be empty");
        }
        
        if (string.IsNullOrEmpty(model.OriginalLink))
        {
            throw new ArgumentException("OriginalLink cannot be empty");
        }
        
        if (string.IsNullOrEmpty(model.Publisher))
        {
            throw new ArgumentException("Publisher cannot be empty");
        }
        
        if (string.IsNullOrEmpty(model.PublisherLink))
        {
            throw new ArgumentException("PublisherLink cannot be empty");
        }
        
        if (string.IsNullOrEmpty(model.Guid))
        {
            throw new ArgumentException("Guid cannot be empty");
        }
    }
    
    public void ValidateBulk(NewsCreateModel[] models)
    {
        foreach (var model in models)
        {
            Validate(model);
        }
    }
}