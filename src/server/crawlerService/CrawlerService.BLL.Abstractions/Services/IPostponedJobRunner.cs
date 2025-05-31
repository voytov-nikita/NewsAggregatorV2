using System.Linq.Expressions;

namespace CrawlerService.BLL.Abstractions.Services;

public interface IPostponedJobRunner
{
    string Enqueue<T>(Expression<Func<T, Task>> methodCall);
}