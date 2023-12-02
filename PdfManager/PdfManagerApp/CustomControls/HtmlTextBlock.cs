using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace PdfManagerApp.CustomControls;

public class HtmlTextBlock : TextBlock
{
    public static readonly DependencyProperty HtmlTextProperty =
        DependencyProperty.Register("HtmlText", typeof(string), typeof(HtmlTextBlock), new PropertyMetadata(null, OnHtmlTextChanged));

    public string HtmlText
    {
        get { return (string)GetValue(HtmlTextProperty); }
        set { SetValue(HtmlTextProperty, value); }
    }

    private static void OnHtmlTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var textBlock = (HtmlTextBlock)d;
        textBlock.Inlines.Clear();
        if (e.NewValue != null)
        {
            textBlock.Inlines.AddRange(ParseInlines(e.NewValue.ToString()));
        }
    }

    private static IEnumerable<Inline> ParseInlines(string text)
    {
        var textBlock = (TextBlock)XamlReader.Parse(
            "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"
            + text
            + "</TextBlock>");

        return textBlock.Inlines.ToList();
    }
}