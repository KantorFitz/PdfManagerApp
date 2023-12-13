using System;
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
    public void Convert_ShouldReturnDescription_WhenEnumWithDescriptionIsGiven()
    {
        // arrange
        var testEnum = TestEnum.One;

        // act
        var result = _converter.Convert(testEnum, null, null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be("Test Description One");
    }

    [Fact]
    public void Convert_ShouldReturnEnumName_WhenEnumWithoutDescriptionIsGiven()
    {
        // arrange
        var testEnum = TestEnum.Two;

        // act
        var result = _converter.Convert(testEnum, null, null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be(Enum.GetName(testEnum));
    }

    [Fact]
    public void Convert_ShouldReturnFalse_WhenNullObjectIsGiven()
    {
        object testObject = null;

        // act
        var result = _converter.Convert(testObject, typeof(string), null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be(false);
    }

    [Fact]
    public void ConvertBack_ShouldReturnUnset_Always()
    {
        object testObject = "test";

        // act
        var result = _converter.ConvertBack(testObject, typeof(string), null, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be(DependencyProperty.UnsetValue);
    }
}
