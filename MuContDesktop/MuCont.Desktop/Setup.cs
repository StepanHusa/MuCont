using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuCont.Desktop.DockingUtilities;
using MuCont.Desktop.ViewModels;
using Prism.Events;
using System;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using MuCont.Desktop.Services;
using MuCont.Desktop.Dialogs.ViewModels;
using MuCont.ComputationInterface;
using MuCont.Desktop.ViewModels.Dockable.Plots;
using MuCont.Desktop.ViewModels.Dockable;
using MuCont.Desktop.Services.ApiServices.SystemsService;
using MuCont.Desktop.Services.ApiServices.ComputationSchedulingService;
using MuCont.Desktop.Services.ApiServices.ApiService;


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

        //The window (Only one main window)
        services.AddSingleton<MainWindow>();
        //services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();


        //ViewModels
        services.AddSingleton<MainViewModel>();


        //ViewModels Transient
        services.AddTransient<SettingsDialogViewModel>();
        services.AddTransient<NewSystemDialogViewModel>();
        services.AddTransient<ErrorDialogViewModel>();

        //Docking
        services.AddSingleton<IDockFactory, DockFactory>();
        services.AddSingleton<DockingContext>();

        services.AddTransient<PlotViewModel>();
        services.AddTransient<StarterViewModel>();
        services.AddTransient<SystemsViewModel>();


        //Api Services
        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IComputationSchedulingService, ComputationSchedulingService>();

        // Other
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<RefreshScheduler>();

        services.AddSingleton<IEventAggregator, EventAggregator>();

        return services.BuildServiceProvider();
    }
}


