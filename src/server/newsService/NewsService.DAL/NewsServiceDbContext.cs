using Microsoft.EntityFrameworkCore;
using NewsService.DAL.Entities;

namespace NewsService.DAL;

public class NewsServiceDbContext(DbContextOptions<NewsServiceDbContext> options) : DbContext(options)
{
    public DbSet<NewsEntity> News { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
};