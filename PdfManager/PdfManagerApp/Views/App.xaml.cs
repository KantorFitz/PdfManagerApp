using System.Globalization;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PdfManagerApp.Extensions;
using PdfManagerApp.Infrastructure;
using PdfManagerApp.ViewModels;

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
        dbContext.Database.Migrate();
        dbContext.Folders.Include(x=>x.BookDetails).Load();

        var savedFoldersMv = serviceProvider.GetRequiredService<SettingsWindowViewModel>();
        savedFoldersMv.Folders = dbContext.Folders.Local.ToObservableCollection();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}