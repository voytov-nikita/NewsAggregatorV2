using CrawlerService.BLL.Abstractions.Services;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace CrawlerService.Hangfire.Services;

public class HangfireSchedulerBackgroundService: BackgroundService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceProvider _serviceProvider;

    public HangfireSchedulerBackgroundService(IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
    {
        _recurringJobManager = recurringJobManager;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Todo: add opportunity of changing schedule by API request

        /*IDataSourceService dataSourceService = new DataSourceService();
        DataSourceModel[] dataSources = dataSourceService.GetAllAsync();

        foreach (var dataSource in dataSources)
        {
            dataSource.ProvidaerName = ProviderType.Pravda;

            INewsParserService parserType = _serviceProvider.GetService(Type.GetType(dataSource.Type)) as INewsParserService;
            _recurringJobManager.AddOrUpdate<INewsDownloaderService>(dataSource.UniqueName, service => service.GetNewsAsync(dataSource, parserType), "#1#30 * * * * *"); // Every 30 seconds.

            _recurringJobManager.RemoveIfExists(dataSource.UniqueName);

            switch (dataSource.Type)
            {
                case "rss":
                    _recurringJobManager.AddOrUpdate<IRssNewsDownloaderService>(dataSource.UniqueName, service => service.GetNewsAsync<IPravdaNewsPraser>(dataSource), "#1#30 * * * * *"); // Every 30 seconds.
                    break;
                case "html":
                    _recurringJobManager.AddOrUpdate<IHtmlNewsDownloaderService>(dataSource.UniqueName, service => service.GetNewsAsync(dataSource), "#1#30 * * * * *"); // Every 30 seconds.
                    break;
            }
        }*/



        _recurringJobManager.AddOrUpdate<INewsDownloaderService>("crawl-news", service => service.GetNewsAsync(), "*/30 * * * * *"); // Every 30 seconds.

        return Task.CompletedTask;
    }
}