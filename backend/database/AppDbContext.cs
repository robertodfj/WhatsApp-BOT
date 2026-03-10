using Bot.Api.Model.Auth;
using Microsoft.EntityFrameworkCore;

namespace Bot.Api.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(x => x.Status)
            .HasConversion<string>();
    }
}
