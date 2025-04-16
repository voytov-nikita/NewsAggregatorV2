using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NewsService.DAL.PostgreSql.Filters;

public class MigrationsFilter : IStartupFilter
{
	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
	{
		return builder =>
		{
			using (var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				scope.ServiceProvider
					.GetRequiredService<NewsServiceDbContext>()
					.Database
					.Migrate();
			}

			next(builder);
		};
	}
}
