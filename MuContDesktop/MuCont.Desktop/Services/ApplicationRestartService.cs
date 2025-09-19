using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services;

public class ApplicationRestartService : IApplicationRestartService
{
    private readonly ILogger<ApplicationRestartService> _logger;

    public ApplicationRestartService(ILogger<ApplicationRestartService> logger)
    {
        _logger = logger;
    }

    public async Task RestartApplicationAsync()
    {
        try
        {
            _logger.LogInformation("Starting application restart process...");

            // Prepare for restart (save any necessary state)
            await PrepareForRestartAsync();

            // Get the current executable path
            var currentExecutable = Environment.ProcessPath ?? GetCurrentExecutablePath();
            
            if (string.IsNullOrEmpty(currentExecutable) || !File.Exists(currentExecutable))
            {
                _logger.LogError("Could not determine current executable path for restart");
                throw new InvalidOperationException("Could not determine current executable path for restart");
            }

            _logger.LogInformation("Restarting application: {ExecutablePath}", currentExecutable);

            // Start the new process
            var startInfo = new ProcessStartInfo
            {
                FileName = currentExecutable,
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            // Copy command line arguments if any
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    startInfo.ArgumentList.Add(args[i]);
                }
            }

            Process.Start(startInfo);

            // Shutdown current application
            await ShutdownCurrentApplicationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart application");
            throw;
        }
    }

    public async Task PrepareForRestartAsync()
    {
        try
        {
            _logger.LogInformation("Preparing application for restart...");
            
            //TODO add a logic that ensures that every process is finished correctly            
            
            _logger.LogInformation("Application prepared for restart");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing application for restart");
            throw;
        }
    }

    private string GetCurrentExecutablePath()
    {
        // Fallback methods to get executable path
        try
        {
            // Try using Process.GetCurrentProcess()
            using var process = Process.GetCurrentProcess();
            var mainModule = process.MainModule;
            if (mainModule?.FileName != null)
            {
                return mainModule.FileName;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not get executable path from current process");
        }

        // Try using AppContext.BaseDirectory
        var baseDir = AppContext.BaseDirectory;
        if (!string.IsNullOrEmpty(baseDir))
        {
            var exeName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) + ".exe";
            var exePath = Path.Combine(baseDir, exeName);
            if (File.Exists(exePath))
            {
                return exePath;
            }
        }

        throw new InvalidOperationException("Could not determine executable path");
    }

    private async Task ShutdownCurrentApplicationAsync()
    {
        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _logger.LogInformation("Shutting down desktop application...");
                
                // Allow some time for the new process to start
                await Task.Delay(500);
                
                // Shutdown the current application
                desktop.Shutdown();
            }
            else
            {
                _logger.LogInformation("Exiting application via Environment.Exit...");
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during application shutdown");
            // Fallback to Environment.Exit
            Environment.Exit(1);
        }
    }
}