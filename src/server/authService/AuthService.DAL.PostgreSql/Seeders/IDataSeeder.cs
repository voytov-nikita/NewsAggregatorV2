namespace AuthService.DAL.PostgreSql.Seeders;

public interface IDataSeeder
{
    /// <summary>Lower runs first.</summary>
    int Order { get; }

    Task SeedAsync(IServiceProvider serviceProvider);
}
