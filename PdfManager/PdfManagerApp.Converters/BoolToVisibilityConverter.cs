﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PdfManagerApp.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.GetType() != typeof(bool))
            return null;

        var boolValue = (bool)value;

        return boolValue ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.GetType() != typeof(Visibility))
            return null;

        return (Visibility)value == Visibility.Visible;
    }
}
