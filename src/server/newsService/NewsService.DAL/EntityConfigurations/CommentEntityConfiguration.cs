using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.Entities;

namespace NewsService.DAL.EntityConfigurations;

public class CommentEntityConfiguration: IEntityTypeConfiguration<CommentEntity>
{
    
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.ToTable("NewsComments");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.LastModifiedDate)
            .IsRequired(false);
    }
}