using System.ComponentModel;

namespace PdfManagerApp.Extensions;

public static class EnumExtensions
{
    public static string? GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo == null)
            return Enum.GetName(value.GetType(), value);

        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes is { Length: > 0 }
            ? attributes[0].Description
            : Enum.GetName(value.GetType(), value);
    }
}
