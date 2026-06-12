using System.Globalization;

namespace SecilStoreConfig.Library.TypeConversion;

public static class ValueConverter
{
    public static T ConvertTo<T>(string type, string value)
    {
        object result = type.ToLower() switch
        {
            "int" => int.Parse(value),
            "bool" => value == "1" || value.ToLower() == "true",
            "double" => double.Parse(value, CultureInfo.InvariantCulture),
            _ => value
        };

        return (T)Convert.ChangeType(result, typeof(T), CultureInfo.InvariantCulture);
    }
}
