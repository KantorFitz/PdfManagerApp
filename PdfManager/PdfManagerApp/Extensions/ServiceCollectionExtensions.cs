using Microsoft.Extensions.DependencyInjection;
using PdfManagerApp.ViewModels;
using PdfManagerApp.Views;

namespace PdfManagerApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddScoped<SettingsWindow>();

        return services;
    }

    public static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<SettingsWindowViewModel>();

        return services;
    }
}