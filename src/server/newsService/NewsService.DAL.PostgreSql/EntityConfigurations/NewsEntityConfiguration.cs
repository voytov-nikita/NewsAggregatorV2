using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.PostgreSql.Entities;

namespace NewsService.DAL.PostgreSql.EntityConfigurations;

public class NewsEntityConfiguration : IEntityTypeConfiguration<NewsEntity>
{
    public void Configure(EntityTypeBuilder<NewsEntity> builder)
    {
        builder.ToTable("News");

        builder.HasKey(x => x.Id);

        builder.Property(_ => _.ImageLink)
            .IsRequired(false);

        builder.Property(_ => _.Description)
            .IsRequired(false);

        builder.HasMany(_ => _.Comments)
            .WithOne(_ => _.News)
            .HasForeignKey(_ => _.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(_ => new { _.Publisher, _.Guid }).IsUnique();
    }
};