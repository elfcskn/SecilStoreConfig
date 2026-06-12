using SecilStoreConfig.Library;
using SecilStoreConfig.Library.Models;

namespace SecilStoreConfig.Library.Tests;

public class ConfigurationReaderTests
{
    private const int LongInterval = 60_000;

    private static ConfigItem Item(string name, string type, string value, string app, bool active = true) =>
        new() { Name = name, Type = type, Value = value, ApplicationName = app, IsActive = active };

    [Fact]
    public async Task GetValueAsync_Returns_Seeded_Value_As_Correct_Type()
    {
        var storage = new FakeRepository(new[]
        {
            Item("SiteName", "string", "soty.io", "SERVICE-A"),
            Item("MaxItemCount", "int", "50", "SERVICE-A"),
        });

        using var reader = new ConfigurationReader("SERVICE-A", storage, LongInterval);

        Assert.Equal("soty.io", await reader.GetValueAsync<string>("SiteName"));
        Assert.Equal(50, await reader.GetValueAsync<int>("MaxItemCount"));
    }

    [Fact]
    public async Task GetValueAsync_Is_Case_Insensitive_On_Key()
    {
        var storage = new FakeRepository(new[] { Item("SiteName", "string", "soty.io", "SERVICE-A") });
        using var reader = new ConfigurationReader("SERVICE-A", storage, LongInterval);

        Assert.Equal("soty.io", await reader.GetValueAsync<string>("sitename"));
    }

    [Fact]
    public async Task GetValueAsync_Throws_For_Unknown_Key()
    {
        var storage = new FakeRepository(new[] { Item("SiteName", "string", "soty.io", "SERVICE-A") });
        using var reader = new ConfigurationReader("SERVICE-A", storage, LongInterval);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => reader.GetValueAsync<string>("DoesNotExist"));
    }

    [Fact]
    public async Task Reader_Only_Sees_Its_Own_Applications_Records()
    {
        var storage = new FakeRepository(new[]
        {
            Item("SiteName", "string", "a-value", "SERVICE-A"),
            Item("SiteName", "string", "b-value", "SERVICE-B"),
        });

        using var readerA = new ConfigurationReader("SERVICE-A", storage, LongInterval);
        Assert.Equal("a-value", await readerA.GetValueAsync<string>("SiteName"));
    }

    [Fact]
    public async Task Inactive_Records_Are_Not_Served()
    {
        var storage = new FakeRepository(new[]
        {
            Item("MaxItemCount", "int", "50", "SERVICE-A", active: false),
        });

        using var reader = new ConfigurationReader("SERVICE-A", storage, LongInterval);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => reader.GetValueAsync<int>("MaxItemCount"));
    }

    [Fact]
    public async Task RefreshAsync_Picks_Up_Changed_Values()
    {
        var storage = new FakeRepository(new[] { Item("SiteName", "string", "soty.io", "SERVICE-A") });
        using var reader = new ConfigurationReader("SERVICE-A", storage, LongInterval);

        Assert.Equal("soty.io", await reader.GetValueAsync<string>("SiteName"));

        storage.SetItems(new[] { Item("SiteName", "string", "secil.io", "SERVICE-A") });
        await reader.RefreshAsync();

        Assert.Equal("secil.io", await reader.GetValueAsync<string>("SiteName"));
    }

    [Fact]
    public async Task Keeps_Last_Good_Values_When_Storage_Fails()
    {
        var storage = new FakeRepository(new[] { Item("SiteName", "string", "soty.io", "SERVICE-A") });
        using var reader = new ConfigurationReader("SERVICE-A", storage, LongInterval);

        Assert.Equal("soty.io", await reader.GetValueAsync<string>("SiteName"));

        storage.ThrowOnNextRead = true;
        await reader.RefreshAsync();

        Assert.Equal("soty.io", await reader.GetValueAsync<string>("SiteName"));
    }

    [Fact]
    public void Constructor_Validates_Arguments()
    {
        var storage = new FakeRepository();
        Assert.Throws<ArgumentException>(() => new ConfigurationReader("", storage, LongInterval));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ConfigurationReader("SERVICE-A", storage, 0));
    }
}
