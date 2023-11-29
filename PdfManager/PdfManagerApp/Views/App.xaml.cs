using System.Globalization;
using System.Text;
using System.Windows;

namespace PdfManagerApp.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("pl");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");
        base.OnStartup(e);
    }
}
