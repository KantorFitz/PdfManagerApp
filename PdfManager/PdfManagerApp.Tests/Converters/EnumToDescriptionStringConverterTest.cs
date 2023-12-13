using System.ComponentModel;
using System.Globalization;
using System.Windows;
using FluentAssertions;
using PdfManagerApp.Converters;
using Xunit;

namespace PdfManagerApp.Tests.Converters;

public enum TestEnum
{
    [Description("Test Description One")]
    One,
    Two,

    [Description("Test Description Three")]
    Three
}

public class EnumToDescriptionStringConverterTest
{
    private readonly EnumToDescriptionStringConverter _converter = new();

    [Fact]
    public void Convert_WithDescription_ShouldReturnDescription()
    {
        // arrange
        var testEnum = TestEnum.One;

        // act
        var result = _converter.Convert(testEnum, null, null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be("Test Description One");
    }

    [Fact]
    public void Convert_WithoutDescription_ShouldReturnEnumName()
    {
        // arrange
        var testEnum = TestEnum.Two;

        // act
        var result = _converter.Convert(testEnum, null, null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be("Two");
    }

    [Fact]
    public void Convert_NullObject_ShouldReturnEmptyString()
    {
        object testObject = null;

        // act
        var result = _converter.Convert(testObject, typeof(string), null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be("");
    }

    [Fact]
    public void ConvertBack_ShouldAlwaysReturnUnset()
    {
        object testObject = "test";

        // act
        var result = _converter.ConvertBack(testObject, typeof(string), null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be(DependencyProperty.UnsetValue);
    }
}
