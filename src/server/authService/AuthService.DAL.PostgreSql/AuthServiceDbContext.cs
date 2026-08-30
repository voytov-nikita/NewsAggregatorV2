using AuthService.DAL.PostgreSql.Entities;
using AuthService.DAL.PostgreSql.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.PostgreSql;

public class AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<PermissionEntity> Permissions { get; set; }

    public DbSet<RolePermissionEntity> RolePermissions { get; set; }

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity builds its own model first; the default schema is applied on top of it.
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("authService");

        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityConfiguration());
    }
}
