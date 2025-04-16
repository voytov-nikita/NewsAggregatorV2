using NewsService.Models.News;

namespace NewsService.BLL.Abstractions.Validators;

public interface INewsValidator
{
    public void Validate(NewsFilterModel filter);
    public void Validate(NewsCreateModel model);
}