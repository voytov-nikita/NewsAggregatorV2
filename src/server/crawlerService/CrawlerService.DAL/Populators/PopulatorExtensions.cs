using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CrawlerService.DAL.Populators;

public static class PopulatorExtensions
{
    /// <summary>
    /// Runs all registered <see cref="IPopulator"/>s against the Mongo database.
    /// Call this between <c>builder.Build()</c> and <c>app.Run()</c> to guarantee
    /// seed data is in place before any hosted service (e.g. Hangfire scheduler) starts.
    /// </summary>
    public static void RunPopulators(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        IMongoDatabase database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        IEnumerable<IPopulator> populators = scope.ServiceProvider.GetServices<IPopulator>();

        foreach (IPopulator populator in populators)
        {
            populator.Populate(database);
        }
    }
}
