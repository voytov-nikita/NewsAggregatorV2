using AuthService.DAL.PostgreSql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.PostgreSql.EntityConfigurations;

public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.TokenHash).IsRequired().HasMaxLength(64);
        builder.Property(_ => _.CreatedByIp).HasMaxLength(64);
        builder.Property(_ => _.UserAgent).HasMaxLength(512);
        builder.Property(_ => _.RevokedReason).HasConversion<short?>();

        builder.HasIndex(_ => _.TokenHash).IsUnique();
        builder.HasIndex(_ => new { _.UserId, _.RevokedAt });
        builder.HasIndex(_ => _.FamilyId);

        builder
            .HasOne(_ => _.User)
            .WithMany()
            .HasForeignKey(_ => _.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
