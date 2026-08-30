namespace AuthService.DAL.PostgreSql.Entities;

/// <summary>
/// Permissions live in real tables instead of AspNetRoleClaims so the catalogue is queryable
/// ("which permissions exist?") and RolePermissions gets a foreign key, which stops a typo
/// from becoming a silent no-op. AspNetRoleClaims/AspNetUserClaims are created by Identity
/// but intentionally unused.
/// </summary>
public class PermissionEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
}
