using SecilStoreConfig.Library.TypeConversion;

namespace SecilStoreConfig.Library.Tests;

public class ValueConverterTests
{
    [Fact]
    public void Converts_String()
    {
        Assert.Equal("soty.io", ValueConverter.ConvertTo<string>("string", "soty.io"));
    }

    [Fact]
    public void Converts_Int()
    {
        Assert.Equal(50, ValueConverter.ConvertTo<int>("int", "50"));
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Converts_Bool_From_NumericAndText(string raw, bool expected)
    {
        Assert.Equal(expected, ValueConverter.ConvertTo<bool>("bool", raw));
    }

    [Fact]
    public void Converts_Double_WithInvariantCulture()
    {
        Assert.Equal(0.18, ValueConverter.ConvertTo<double>("double", "0.18"));
    }

    [Fact]
    public void Coerces_Int_To_String_When_T_Is_String()
    {
        Assert.Equal("50", ValueConverter.ConvertTo<string>("int", "50"));
    }
}
