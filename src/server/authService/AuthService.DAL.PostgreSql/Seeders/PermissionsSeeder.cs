using Common.Auth.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.DAL.PostgreSql.Seeders;

/// <summary>
/// Syncs the Permissions table from <see cref="Permissions.All"/>. Code is the source of truth,
/// the table is the join target - which is also why this is a startup seeder rather than
/// migration HasData: adding a permission must not require a new migration.
/// </summary>
public class PermissionsSeeder : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        AuthServiceDbContext context = serviceProvider.GetRequiredService<AuthServiceDbContext>();

        string[] expected = Permissions.All;
        List<Entities.PermissionEntity> existing = await context.Permissions.ToListAsync();

        foreach (string name in expected.Except(existing.Select(_ => _.Name)))
        {
            context.Permissions.Add(new Entities.PermissionEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
            });
        }

        Entities.PermissionEntity[] obsolete = existing.Where(_ => !expected.Contains(_.Name)).ToArray();

        if (obsolete.Length > 0)
        {
            context.Permissions.RemoveRange(obsolete);
        }

        await context.SaveChangesAsync();
    }
}
