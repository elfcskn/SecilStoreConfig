using SecilStoreConfig.Library;
using SecilStoreConfig.Library.Data;

const string applicationName = "SERVICE-A";  //Uygulama SERVICE-A olarak taniyor
const int refreshIntervalMs = 3000;

await DbInitializer.InitializeAsync(SharedDatabase.ConnectionString);

using var reader = new ConfigurationReader(
    applicationName,
    SharedDatabase.ConnectionString,
    refreshIntervalMs);

Console.WriteLine($"=== Config Demo ({applicationName}) ===");
Console.WriteLine("Edit a SERVICE-A value in the web panel and watch it change here.");
Console.WriteLine("Press Ctrl+C to exit.\n");

while (true)
{
    try
    {
        var siteName = await reader.GetValueAsync<string>("SiteName");
        var maxItems = await reader.GetValueAsync<int>("MaxItemCount");
        var taxRate = await reader.GetValueAsync<double>("TaxRate");

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] SiteName={siteName} | MaxItemCount={maxItems} | TaxRate={taxRate}");
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {ex.Message}");
    }

    await Task.Delay(refreshIntervalMs);
}
