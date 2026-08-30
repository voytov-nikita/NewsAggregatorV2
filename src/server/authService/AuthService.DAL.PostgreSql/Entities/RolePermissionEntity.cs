namespace AuthService.DAL.PostgreSql.Entities;

public class RolePermissionEntity
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public ApplicationRole Role { get; set; } = null!;

    public PermissionEntity Permission { get; set; } = null!;
}
