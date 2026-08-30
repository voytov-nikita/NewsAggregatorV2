using AuthService.DAL.Abstractions.Stores;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.PostgreSql.Stores;

public class PermissionsStore(AuthServiceDbContext context) : IPermissionsStore
{
    public async Task<IReadOnlyCollection<string>> GetAllNamesAsync() =>
        await context.Permissions.Select(_ => _.Name).ToArrayAsync();

    public async Task<IReadOnlyCollection<string>> GetForRolesAsync(IReadOnlyCollection<string> roleNames)
    {
        if (roleNames.Count == 0)
        {
            return [];
        }

        return await context.RolePermissions
            .Where(_ => roleNames.Contains(_.Role.Name!))
            .Select(_ => _.Permission.Name)
            .Distinct()
            .ToArrayAsync();
    }
}
