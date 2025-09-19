using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuCont.ComputationInterface;
using MuCont.Desktop.Dialogs.ViewModels;
using MuCont.Desktop.DockingUtilities;
using MuCont.Desktop.Services;
using MuCont.Desktop.Services.ApiServices.ApiService;
using MuCont.Desktop.Services.ApiServices.ComputationSchedulingService;
using MuCont.Desktop.Services.ApiServices.SystemsService;
using MuCont.Desktop.ViewModels;
using MuCont.Desktop.ViewModels.Dockable;
using MuCont.Desktop.ViewModels.Dockable.Plots;
using Prism.Events;
using Serilog;
using System;
using System.IO;


namespace MuCont.Desktop;

public static class Startup
{
    public static IServiceProvider ConfigureServices()
    {
        // Configure Serilog for file-only logging
        ConfigureLogging();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // Add logging integration
        services.AddLogging(builder => builder.AddSerilog());

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

        // File Service
        services.AddSingleton<IFileService, FileService>();

        // Other
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<RefreshScheduler>();

        services.AddSingleton<IEventAggregator, EventAggregator>();

        return services.BuildServiceProvider();
    }

    private static void ConfigureLogging()
    {
        // Get the log directory using PathManager
        var logDirectory = PathManager.GetDesktopLogsDirectory();

        // Ensure log directory exists
        PathManager.EnsureDirectoryExists(logDirectory);

        var logFilePath = Path.Combine(logDirectory, "app-.txt");

        // Configure Serilog for file-only logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .CreateLogger();

        var pathInfo = PathManager.GetPathInfo();
        Log.Information("MuCont Desktop application starting...");
        Log.Information("Platform: {Platform}", pathInfo.Platform);
        Log.Information("Using custom path: {IsCustomPath}", pathInfo.IsUsingCustomPath);
        Log.Information("Log files directory: {LogDirectory}", logDirectory);
        Log.Information("Base config directory: {BaseDirectory}", pathInfo.BaseConfigDirectory);
    }
}


