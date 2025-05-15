using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.PostgreSql.Entities;

namespace NewsService.DAL.PostgreSql;

public class NewsServiceDbContext(DbContextOptions<NewsServiceDbContext> options) : DbContext(options)
{
    public DbSet<NewsEntity> News { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("newsService");
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new NewsEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());
    }
};

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
    }
};


public class CommentEntityConfiguration : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.ToTable("Comments");
        
        builder.HasKey(x => x.Id);

        builder.Property(_ => _.LastModifiedDate)
            .IsRequired(false);
    }
};