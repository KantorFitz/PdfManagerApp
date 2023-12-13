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
    private ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>();

        services.RegisterViews();
        services.RegisterViewModels();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var dbContext = _serviceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.Migrate();
        dbContext.Folders.Include(x=>x.BookDetails).Load();

        var savedFoldersViewModel = _serviceProvider.GetRequiredService<SettingsWindowViewModel>();
        savedFoldersViewModel.Folders = dbContext.Folders.Local.ToObservableCollection();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}