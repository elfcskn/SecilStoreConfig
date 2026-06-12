using Microsoft.EntityFrameworkCore;
using SecilStoreConfig.Library.Models;

namespace SecilStoreConfig.Library.Data;

public static class DbInitializer  //Veritabaný oluþumu 
{
    public static async Task InitializeAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ConfigDbContext>()
            .UseSqlite(connectionString)
            .Options;

        await using var db = new ConfigDbContext(options);

        await db.Database.EnsureCreatedAsync();

        await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");

        if (!await db.Configurations.AnyAsync())
        {
            db.Configurations.AddRange(GetSeedData());
            await db.SaveChangesAsync();
        }
    }

    private static IEnumerable<ConfigItem> GetSeedData() => new[]
    {
        new ConfigItem { Name = "SiteName",        Type = "string", Value = "soty.io", IsActive = true,  ApplicationName = "SERVICE-A" },
        new ConfigItem { Name = "IsBasketEnabled", Type = "bool",   Value = "1",       IsActive = true,  ApplicationName = "SERVICE-B" },
        new ConfigItem { Name = "MaxItemCount",    Type = "int",    Value = "50",      IsActive = false, ApplicationName = "SERVICE-A" },
        new ConfigItem { Name = "MaxItemCount",    Type = "int",    Value = "100",     IsActive = true,  ApplicationName = "SERVICE-A" },
        new ConfigItem { Name = "TaxRate",         Type = "double", Value = "0.18",    IsActive = true,  ApplicationName = "SERVICE-A" },
    };
}
