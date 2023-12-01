using System.Globalization;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PdfManagerApp.Extensions;
using PdfManagerApp.Infrastructure;

namespace PdfManagerApp.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>();

        services.RegisterViews();
        services.RegisterViewModels();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("pl");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");

        var dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureCreated();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}