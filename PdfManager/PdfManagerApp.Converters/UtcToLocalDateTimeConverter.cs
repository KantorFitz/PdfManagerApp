using System.Globalization;
using System.Windows.Data;

namespace PdfManagerApp.Converters;

public class UtcToLocalDateTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (TryConvertToDateTime(value, out var dateTime))
            return dateTime!.Value.ToLocalTime();

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (TryConvertToDateTime(value, out var dateTime))
            return dateTime!.Value.ToUniversalTime();

        return null;
    }

    private static bool TryConvertToDateTime(object value, out DateTime? dateTime)
    {
        if (value == null)
        {
            dateTime = null;
            return false;
        }

        if (value is not DateTime dt)
        {
            dateTime = null;
            return false;
        }

        dateTime = dt;
        return true;
    }
}