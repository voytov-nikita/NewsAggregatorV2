using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MongoDb;
using Xunit;

namespace CrawlerService.DAL.IntegrationTests;

public class EnvironmentFixture: IAsyncLifetime
{
	private readonly MongoDbContainer _dbContainer;

	public EnvironmentFixture()
	{
		_dbContainer = new MongoDbBuilder()
			.WithName(Constants.DatabaseName)
			.Build();
	}

	public ServiceProvider BuildServiceProvider()
	{
		IServiceCollection services = new ServiceCollection();
		//Logging to console for tests
		services.AddLogging(builder => builder.AddConsole());

		DatabaseSettings settings = new DatabaseSettings()
		{
			ConnectionString = _dbContainer.GetConnectionString(),
			DatabaseName = Constants.DatabaseName,
		};
		services.AddDataAccessLayer(settings);

		ServiceProvider serviceProvider = services.BuildServiceProvider();
		return serviceProvider;
	}

	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();
	}

	public async Task DisposeAsync()
	{
		await _dbContainer
			.DisposeAsync()
			.AsTask();
	}
}
