using SecilStoreConfig.Library.Models;

namespace SecilStoreConfig.Library.Repositories;

public interface IConfigurationRepository
{
    Task<IReadOnlyList<ConfigItem>> GetActiveAsync(
        string applicationName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConfigItem>> GetAllAsync();
    Task<ConfigItem?> GetByIdAsync(int id);
    Task AddAsync(ConfigItem item);
    Task UpdateAsync(ConfigItem item);
    Task DeleteAsync(int id);
}
