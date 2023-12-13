using System.Globalization;
using System.Windows;
using FluentAssertions;
using PdfManagerApp.Converters;
using Xunit;

namespace PdfManagerApp.Tests.Converters;

public class BoolToVisibilityConverterTest
{
    private readonly BoolToVisibilityConverter _converter = new();

    [Fact]
    public void Convert_WhenTrue_ShouldReturnVisible()
    {
        _converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture)
            .Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_WhenFalse_ShouldReturnHidden()
    {
        _converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture)
            .Should().Be(Visibility.Hidden);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("nonBoolValue")]
    public void Convert_WhenNotBool_ShouldReturnNull(object value)
    {
        _converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture)
            .Should().Be(null);
    }

    [Fact]
    public void ConvertBack_WhenVisible_ShouldReturnTrue()
    {
        _converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture)
            .Should().Be(true);
    }

    [Fact]
    public void ConvertBack_WhenHidden_ShouldReturnFalse()
    {
        _converter.ConvertBack(Visibility.Hidden, typeof(bool), null, CultureInfo.InvariantCulture)
            .Should().Be(false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("nonVisibilityValue")]
    public void ConvertBack_WhenNotVisibility_ShouldReturnNull(object value)
    {
        _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture)
            .Should().Be(null);
    }
}
