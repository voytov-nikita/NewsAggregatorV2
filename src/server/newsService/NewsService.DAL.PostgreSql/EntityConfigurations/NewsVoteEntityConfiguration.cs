using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.PostgreSql.Entities;

namespace NewsService.DAL.PostgreSql.EntityConfigurations;

public class NewsVoteEntityConfiguration : IEntityTypeConfiguration<NewsVoteEntity>
{
    public void Configure(EntityTypeBuilder<NewsVoteEntity> builder)
    {
        builder.ToTable("Votes");

        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.Value).IsRequired();
        builder.Property(_ => _.LastModifiedDate).IsRequired(false);

        // The database, not the application, is what guarantees one vote per user per article.
        builder.HasIndex(_ => new { _.NewsId, _.UserId }).IsUnique();

        builder.HasOne(_ => _.News)
            .WithMany(_ => _.Votes)
            .HasForeignKey(_ => _.NewsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
};
