using Microsoft.EntityFrameworkCore;
using SecilStoreConfig.Library.Models;

namespace SecilStoreConfig.Library.Data;

public class ConfigDbContext : DbContext
{
    public ConfigDbContext(DbContextOptions<ConfigDbContext> options) : base(options)
    {
    }

    public DbSet<ConfigItem> Configurations => Set<ConfigItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigItem>().ToTable("Configurations");
    }
}
