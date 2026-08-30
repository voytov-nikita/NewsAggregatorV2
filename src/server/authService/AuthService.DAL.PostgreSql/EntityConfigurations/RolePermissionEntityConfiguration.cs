using AuthService.DAL.PostgreSql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.PostgreSql.EntityConfigurations;

public class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
{
    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(_ => new { _.RoleId, _.PermissionId });

        builder
            .HasOne(_ => _.Role)
            .WithMany()
            .HasForeignKey(_ => _.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(_ => _.Permission)
            .WithMany(_ => _.RolePermissions)
            .HasForeignKey(_ => _.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
