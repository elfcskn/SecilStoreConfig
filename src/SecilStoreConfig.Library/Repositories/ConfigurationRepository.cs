using Microsoft.EntityFrameworkCore;
using SecilStoreConfig.Library.Data;
using SecilStoreConfig.Library.Models;

namespace SecilStoreConfig.Library.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    private readonly DbContextOptions<ConfigDbContext> _options;

    public ConfigurationRepository(string connectionString)
    {
        _options = new DbContextOptionsBuilder<ConfigDbContext>()
            .UseSqlite(connectionString)
            .Options;
    }

    public async Task<IReadOnlyList<ConfigItem>> GetActiveAsync(
        string applicationName,
        CancellationToken cancellationToken = default)
    {
        await using var db = new ConfigDbContext(_options);
        return await db.Configurations
            .AsNoTracking()
            .Where(c => c.ApplicationName == applicationName && c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConfigItem>> GetAllAsync()
    {
        await using var db = new ConfigDbContext(_options);
        return await db.Configurations
            .AsNoTracking()
            .OrderBy(c => c.ApplicationName)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ConfigItem?> GetByIdAsync(int id)
    {
        await using var db = new ConfigDbContext(_options);
        return await db.Configurations.FindAsync(id);
    }

    public async Task AddAsync(ConfigItem item)
    {
        await using var db = new ConfigDbContext(_options);
        db.Configurations.Add(item);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ConfigItem item)
    {
        await using var db = new ConfigDbContext(_options);
        db.Configurations.Update(item);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = new ConfigDbContext(_options);
        var existing = await db.Configurations.FindAsync(id);
        if (existing is null) return;
        db.Configurations.Remove(existing);
        await db.SaveChangesAsync();
    }
}
