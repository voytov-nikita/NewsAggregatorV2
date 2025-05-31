using System.Linq.Expressions;
using CrawlerService.BLL.Abstractions.Services;
using Hangfire;

namespace CrawlerService.Hangfire.Services;

internal class PostponedJobRunner : IPostponedJobRunner
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public PostponedJobRunner(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue<T>(methodCall);
    }
}