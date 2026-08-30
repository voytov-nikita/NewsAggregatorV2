using AuthService.DAL.PostgreSql.Entities;
using Common.Auth.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.DAL.PostgreSql.Seeders;

public class RolesSeeder : IDataSeeder
{
    /// <summary>Which permissions each role carries. Admin gets everything.</summary>
    private static readonly Dictionary<string, string[]> RolePermissions = new()
    {
        [Roles.User] = [Permissions.NewsVote, Permissions.CommentWrite],
        [Roles.Admin] = Permissions.All,
    };

    public int Order => 2;

    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        AuthServiceDbContext context = serviceProvider.GetRequiredService<AuthServiceDbContext>();

        foreach ((string roleName, string[] permissionNames) in RolePermissions)
        {
            ApplicationRole? role = await roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                role = new ApplicationRole { Id = Guid.NewGuid(), Name = roleName };
                await roleManager.CreateAsync(role);
            }

            Guid[] expectedIds = await context.Permissions
                .Where(_ => permissionNames.Contains(_.Name))
                .Select(_ => _.Id)
                .ToArrayAsync();

            List<RolePermissionEntity> current = await context.RolePermissions
                .Where(_ => _.RoleId == role.Id)
                .ToListAsync();

            foreach (Guid permissionId in expectedIds.Except(current.Select(_ => _.PermissionId)))
            {
                context.RolePermissions.Add(new RolePermissionEntity
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                });
            }

            RolePermissionEntity[] obsolete = current
                .Where(_ => !expectedIds.Contains(_.PermissionId))
                .ToArray();

            if (obsolete.Length > 0)
            {
                context.RolePermissions.RemoveRange(obsolete);
            }
        }

        await context.SaveChangesAsync();
    }
}
