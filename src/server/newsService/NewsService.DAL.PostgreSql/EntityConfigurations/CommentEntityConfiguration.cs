using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.PostgreSql.Entities;

namespace NewsService.DAL.PostgreSql.EntityConfigurations;


public class CommentEntityConfiguration : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.ToTable("Comments");
        
        builder.HasKey(x => x.Id);

        builder.Property(_ => _.LastModifiedDate)
            .IsRequired(false);

        builder.Property(_ => _.AuthorName)
            .HasMaxLength(256)
            .IsRequired();

        // "Everything this user wrote" is the query behind both moderation and account deletion.
        builder.HasIndex(_ => _.AuthorId);
    }
};
