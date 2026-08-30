using AuthService.DAL.PostgreSql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.PostgreSql.EntityConfigurations;

public class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.Name).IsRequired().HasMaxLength(128);
        builder.Property(_ => _.Description).HasMaxLength(512);

        builder.HasIndex(_ => _.Name).IsUnique();
    }
}
