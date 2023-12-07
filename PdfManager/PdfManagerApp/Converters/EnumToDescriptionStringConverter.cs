using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;

namespace PdfManagerApp.Converters;

public class EnumToDescriptionStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var description = value.ToString();
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo == null)
            return description;

        var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
        if (attrs is { Length: > 0 })
            description = ((DescriptionAttribute)attrs[0]).Description;

        return description;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}