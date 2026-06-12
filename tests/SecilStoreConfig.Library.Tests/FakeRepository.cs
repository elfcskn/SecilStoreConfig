using SecilStoreConfig.Library.Models;
using SecilStoreConfig.Library.Repositories;

namespace SecilStoreConfig.Library.Tests;

public class FakeRepository : IConfigurationRepository
{
    private List<ConfigItem> _items;

    public bool ThrowOnNextRead { get; set; }

    public FakeRepository(IEnumerable<ConfigItem>? items = null) =>
        _items = items?.ToList() ?? new List<ConfigItem>();

    public void SetItems(IEnumerable<ConfigItem> items) => _items = items.ToList();

    public Task<IReadOnlyList<ConfigItem>> GetActiveAsync(
        string applicationName,
        CancellationToken cancellationToken = default)
    {
        if (ThrowOnNextRead)
        {
            ThrowOnNextRead = false;
            throw new InvalidOperationException("Simulated storage outage.");
        }

        IReadOnlyList<ConfigItem> result = _items
            .Where(c => c.ApplicationName == applicationName && c.IsActive)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<ConfigItem>> GetAllAsync()
    {
        IReadOnlyList<ConfigItem> result = _items.ToList();
        return Task.FromResult(result);
    }

    public Task<ConfigItem?> GetByIdAsync(int id) =>
        Task.FromResult(_items.FirstOrDefault(c => c.Id == id));

    public Task AddAsync(ConfigItem item)
    {
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ConfigItem item)
    {
        _items.RemoveAll(c => c.Id == item.Id);
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _items.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}
