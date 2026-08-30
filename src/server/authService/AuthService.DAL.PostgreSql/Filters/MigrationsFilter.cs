using AuthService.DAL.PostgreSql.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.DAL.PostgreSql.Filters;

public class MigrationsFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            using (var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<AuthServiceDbContext>()
                    .Database
                    .Migrate();

                IOrderedEnumerable<IDataSeeder> seeders = scope.ServiceProvider
                    .GetServices<IDataSeeder>()
                    .OrderBy(_ => _.Order);

                foreach (IDataSeeder seeder in seeders)
                {
                    seeder.SeedAsync(scope.ServiceProvider).GetAwaiter().GetResult();
                }
            }

            next(builder);
        };
    }
}
