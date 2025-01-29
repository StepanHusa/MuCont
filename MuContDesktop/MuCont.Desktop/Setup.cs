using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuCont.ComputationInterface;
using MuCont.Desktop.DockingUtilities;
using MuCont.Desktop.ViewModels;
using Prism.Events;
using System;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using MuCont.Desktop.Services;


namespace MuCont.Desktop;

public static class Startup
{
    public static IServiceProvider ConfigureServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        services.AddSingleton<IEventAggregator, EventAggregator>();

        //The window
        services.AddTransient<MainWindow>();

        //ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<SettingsViewModel>();

        // Other
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<RefreshScheduler>();

        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<IDockFactory, DockFactory>();

        return services.BuildServiceProvider();
    }
}
