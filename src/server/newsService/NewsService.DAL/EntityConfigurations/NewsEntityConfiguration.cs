using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.Entities;

namespace NewsService.DAL.EntityConfigurations;

public class NewsEntityConfiguration: IEntityTypeConfiguration<NewsEntity>
{
    public void Configure(EntityTypeBuilder<NewsEntity> builder)
    {
        builder.ToTable("News");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ImageLink).IsRequired(false);
        
        builder.HasMany(_ => _.Comments)
            .WithOne(x => x.News)
            .HasForeignKey(x => x.NewsId)
            .IsRequired();
    }
}