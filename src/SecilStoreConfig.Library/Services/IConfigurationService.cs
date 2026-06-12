using SecilStoreConfig.Library.Models;

namespace SecilStoreConfig.Library.Services;

public interface IConfigurationService
{
    Task<IReadOnlyList<ConfigItem>> GetAllAsync();
    Task<ConfigItem?> GetByIdAsync(int id);
    Task CreateAsync(ConfigItem item);
    Task UpdateAsync(ConfigItem item);
    Task DeleteAsync(int id);
}
