using System.Collections.Concurrent;
using SecilStoreConfig.Library.Models;
using SecilStoreConfig.Library.Repositories;
using SecilStoreConfig.Library.TypeConversion;

namespace SecilStoreConfig.Library;

public sealed class ConfigurationReader : IDisposable
{
    private readonly string _applicationName;
    private readonly IConfigurationRepository _repository;
    private readonly Timer _timer;

    private volatile ConcurrentDictionary<string, ConfigItem> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    //eklenmesi istenilen kutuphane
    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        : this(applicationName, new ConfigurationRepository(connectionString), refreshTimerIntervalInMs)
    {
    } 

    public ConfigurationReader(string applicationName, IConfigurationRepository repository, int refreshTimerIntervalInMs)
    {
        if (string.IsNullOrWhiteSpace(applicationName))
            throw new ArgumentException("Application name is required.", nameof(applicationName));
        if (refreshTimerIntervalInMs <= 0)
            throw new ArgumentOutOfRangeException(nameof(refreshTimerIntervalInMs));

        _applicationName = applicationName;
        _repository = repository;

        RefreshAsync().GetAwaiter().GetResult();
        _timer = new Timer(async _ => await RefreshAsync(), null, refreshTimerIntervalInMs, refreshTimerIntervalInMs);
    }

    public Task<T> GetValueAsync<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var item))
            throw new KeyNotFoundException($"Configuration '{key}' not found for {_applicationName}.");

        return Task.FromResult(ValueConverter.ConvertTo<T>(item.Type, item.Value));
    }

    public async Task RefreshAsync()
    {
        try
        {
            var items = await _repository.GetActiveAsync(_applicationName);

            var cache = new ConcurrentDictionary<string, ConfigItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
                cache[item.Name] = item;

            _cache = cache;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Config refresh failed, using last known values. {ex.Message}");
        }
    }

    public void Dispose() => _timer?.Dispose();
}
