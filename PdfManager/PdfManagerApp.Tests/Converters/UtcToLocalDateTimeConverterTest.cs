using System;
using System.Globalization;
using FluentAssertions;
using PdfManagerApp.Converters;
using Xunit;

namespace PdfManagerApp.Tests.Converters;

public class UtcToLocalDateTimeConverterTest
{
    private readonly UtcToLocalDateTimeConverter _converter = new();

    [Fact]
    public void Convert_WhenCalledWithNullParameter_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(DateTime), null, CultureInfo.InvariantCulture);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    public void Convert_WhenCalledWithNonDateTimeType_ReturnsNull(Type type)
    {
        var result = _converter.Convert("invalidDatetime", type, null, CultureInfo.InvariantCulture);
        result.Should().BeNull();
    }

    [Fact]
    public void Convert_WhenCalledWithDateTime_ReturnsLocalDateTime()
    {
        var datetime = DateTime.UtcNow;
        var result = _converter.Convert(datetime, typeof(DateTime), null, CultureInfo.InvariantCulture);
        result.Should().Be(datetime.ToLocalTime());
    }

    [Fact]
    public void ConvertBack_WhenCalledWithNullParameter_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(DateTime), null, CultureInfo.InvariantCulture);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    public void ConvertBack_WhenCalledWithNonDateTimeType_ReturnsNull(Type type)
    {
        var result = _converter.ConvertBack("invalidDatetime", type, null, CultureInfo.InvariantCulture);
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertBack_WhenCalledWithDateTime_ReturnsUTCDateTime()
    {
        var datetime = DateTime.Now;
        var result = _converter.ConvertBack(datetime, typeof(DateTime), null, CultureInfo.InvariantCulture);
        result.Should().Be(datetime.ToUniversalTime());
    }
}
