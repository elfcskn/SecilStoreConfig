namespace SecilStoreConfig.Library.Data;

public static class SharedDatabase   //Veritabaný bilgisini tek merkezden iletmek, baðlantý yolu her yerde tekrar yazýlmaz
{
    public static string FilePath
    {
        get
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SecilStoreConfig");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "config.db");
        }
    }

    public static string ConnectionString => $"Data Source={FilePath}";
}
