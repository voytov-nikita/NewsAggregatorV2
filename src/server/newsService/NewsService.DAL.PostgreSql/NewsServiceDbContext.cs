using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DAL.PostgreSql.Entities;
using NewsService.DAL.PostgreSql.EntityConfigurations;

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