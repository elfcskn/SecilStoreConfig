using System.Globalization;
using SecilStoreConfig.Library.Models;
using SecilStoreConfig.Library.Repositories;

namespace SecilStoreConfig.Library.Services;

public class ConfigurationService : IConfigurationService
{
    private static readonly string[] AllowedTypes = { "string", "int", "bool", "double" };

    private readonly IConfigurationRepository _repository;

    public ConfigurationService(IConfigurationRepository repository) => _repository = repository;

    public Task<IReadOnlyList<ConfigItem>> GetAllAsync() => _repository.GetAllAsync();

    public Task<ConfigItem?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task CreateAsync(ConfigItem item)
    {
        Validate(item);
        return _repository.AddAsync(item);
    }

    public Task UpdateAsync(ConfigItem item)
    {
        Validate(item);
        return _repository.UpdateAsync(item);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static void Validate(ConfigItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(item.ApplicationName))
            throw new ArgumentException("ApplicationName is required.");

        item.Type = (item.Type ?? string.Empty).Trim().ToLower();
        if (!AllowedTypes.Contains(item.Type))
            throw new ArgumentException($"Type must be one of: {string.Join(", ", AllowedTypes)}.");

        if (!IsValueValid(item.Type, item.Value))
            throw new ArgumentException($"Value '{item.Value}' is not a valid {item.Type}.");
    }

    private static bool IsValueValid(string type, string value) => type switch
    {
        "int" => int.TryParse(value, out _),
        "double" => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _),
        "bool" => value is "0" or "1" || bool.TryParse(value, out _),
        _ => true
    };
}
