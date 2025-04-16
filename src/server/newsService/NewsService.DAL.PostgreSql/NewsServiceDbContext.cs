using Microsoft.EntityFrameworkCore;
using NewsService.DAL.PostgreSql.Entities;

namespace NewsService.DAL.PostgreSql;

public class NewsServiceDbContext(DbContextOptions<NewsServiceDbContext> options) : DbContext(options)
{
    public DbSet<NewsEntity> News { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
};