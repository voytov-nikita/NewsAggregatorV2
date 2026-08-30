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

        builder.Property(_ => _.Category)
            .HasConversion<short>()
            .HasDefaultValue(Common.Models.NewsCategory.Uncategorized);

        builder.Property(_ => _.ReadTimeMinutes).HasDefaultValue(0);
        builder.Property(_ => _.Likes).HasDefaultValue(0);
        builder.Property(_ => _.Dislikes).HasDefaultValue(0);

        builder.HasIndex(_ => _.Category);

        builder.HasMany(_ => _.Comments)
            .WithOne(_ => _.News)
            // The foreign key is NewsId, not the comment's own primary key. Pointing it at Id made
            // Comments.Id both identity and foreign key, so every insert failed with "The value of
            // CommentEntity.Id is unknown when attempting to save changes".
            .HasForeignKey(_ => _.NewsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(_ => new { _.Publisher, _.Guid }).IsUnique();
    }
};