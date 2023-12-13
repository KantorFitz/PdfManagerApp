using System.Globalization;
using System.Windows.Data;
using System.Windows;
using PdfManagerApp.Extensions;

namespace PdfManagerApp.Converters;

public class EnumToDescriptionStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || (value.GetType() != typeof(Enum) && value.GetType().BaseType != typeof(Enum)))
            return false;

        var enumInstance = (Enum)value;
        return enumInstance.GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}
