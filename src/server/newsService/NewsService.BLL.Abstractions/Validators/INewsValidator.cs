using NewsService.Models.News;
using NewsService.Models.News.Models;

namespace NewsService.BLL.Abstractions.Validators;

public interface INewsValidator
{
    public void Validate(NewsFilterModel filter);
    public void Validate(NewsCreateModel model);
    public void ValidateBulk(NewsCreateModel[] models);
}