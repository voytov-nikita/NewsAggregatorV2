using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit.Categories;
using Xunit.Extensions.AssemblyFixture;

namespace CrawlerService.DAL.IntegrationTests;

[IntegrationTest]
public abstract class BaseTest: IAssemblyFixture<EnvironmentFixture>
{
	private readonly IServiceScope _scope;

	protected BaseTest(EnvironmentFixture fixture)
	{
		_scope = fixture.BuildServiceProvider()
			.CreateScope();
	}

	public T GetService<T>()
	{
		return _scope.ServiceProvider.GetService<T>();
	}

	internal async Task<T> ExecuteDbOperationAsync<T>(Func<IMongoDatabase, Task<T>> action)
	{
		var client = GetService<IMongoClient>();
		var database = client.GetDatabase(Constants.DatabaseName);

		T result = await action(database);

		return result;
	}

	internal Task ExecuteDbOperationAsync(Func<IMongoDatabase, Task> action)
	{
		var client = GetService<IMongoClient>();
		var database = client.GetDatabase(Constants.DatabaseName);

		return action(database);
	}
}
