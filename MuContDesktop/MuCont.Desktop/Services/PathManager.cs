using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MuCont.Desktop.Services;

/// <summary>
/// Centralized path management for the MuCont Desktop application.
/// Handles cross-platform directory resolution with environment variable override support.
/// </summary>
public static class PathManager
{
    private static readonly string ApplicationName = "MuCont";
    private static readonly string DesktopModuleName = "Desktop";

    /// <summary>
    /// Environment variable to override the base configuration directory
    /// </summary>
    public const string ConfigDirectoryEnvironmentVariable = "MUCONT_CONFIG_DIR";

    #region Base Directories

    /// <summary>
    /// Gets the base MuCont configuration directory (cross-platform)
    /// </summary>
    /// <returns>Base configuration path for MuCont</returns>
    public static string GetBaseConfigDirectory()
    {
        // Check for environment variable override first
        var customPath = Environment.GetEnvironmentVariable(ConfigDirectoryEnvironmentVariable);
        if (!string.IsNullOrEmpty(customPath))
        {
            return Path.Combine(customPath, ApplicationName);
        }

        // Use platform-specific default paths
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows: %APPDATA%\MuCont
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS: ~/Library/Application Support/MuCont
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDir, "Library", "Application Support", ApplicationName);
        }
        else // Linux and other Unix-like systems
        {
            // Linux: ~/.config/MuCont (or $XDG_CONFIG_HOME/MuCont)
            var xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (!string.IsNullOrEmpty(xdgConfigHome))
            {
                return Path.Combine(xdgConfigHome, ApplicationName);
            }

            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDir, ".config", ApplicationName);
        }
    }

    /// <summary>
    /// Gets the Desktop module base directory
    /// </summary>
    public static string GetDesktopModuleDirectory()
    {
        return Path.Combine(GetBaseConfigDirectory(), DesktopModuleName);
    }

    #endregion

    #region Desktop Module Paths

    /// <summary>
    /// Gets the Desktop module logs directory
    /// </summary>
    public static string GetDesktopLogsDirectory()
    {
        return Path.Combine(GetDesktopModuleDirectory(), "logs");
    }

    /// <summary>
    /// Gets the Desktop module settings directory
    /// </summary>
    public static string GetDesktopSettingsDirectory()
    {
        return GetDesktopModuleDirectory();
    }

    /// <summary>
    /// Gets the full path for a Desktop module settings file
    /// </summary>
    /// <param name="fileName">Settings file name (e.g., "settings.json")</param>
    public static string GetDesktopSettingsFilePath(string fileName)
    {
        return Path.Combine(GetDesktopSettingsDirectory(), fileName);
    }

    /// <summary>
    /// Gets the Desktop module cache directory
    /// </summary>
    public static string GetDesktopCacheDirectory()
    {
        return Path.Combine(GetDesktopModuleDirectory(), "cache");
    }

    /// <summary>
    /// Gets the Desktop module temp directory
    /// </summary>
    public static string GetDesktopTempDirectory()
    {
        return Path.Combine(GetDesktopModuleDirectory(), "temp");
    }

    #endregion

    #region System Paths

    // /// <summary>
    // /// Gets the user's Documents directory
    // /// </summary>
    // public static string GetUserDocumentsDirectory()
    // {
    //     return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    // }

    // /// <summary>
    // /// Gets the system temp directory
    // /// </summary>
    // public static string GetSystemTempDirectory()
    // {
    //     return Path.GetTempPath();
    // }

    /// <summary>
    /// Gets the application's installation directory
    /// </summary>
    public static string GetApplicationDirectory()
    {
        return AppContext.BaseDirectory;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Ensures a directory exists, creating it if necessary
    /// </summary>
    /// <param name="directoryPath">Directory path to ensure</param>
    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /// <summary>
    /// Gets information about the current path configuration
    /// </summary>
    public static PathInfo GetPathInfo()
    {
        var hasCustomPath = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ConfigDirectoryEnvironmentVariable));

        return new PathInfo
        {
            IsUsingCustomPath = hasCustomPath,
            CustomPathVariable = ConfigDirectoryEnvironmentVariable,
            BaseConfigDirectory = GetBaseConfigDirectory(),
            DesktopModuleDirectory = GetDesktopModuleDirectory(),
            Platform = GetCurrentPlatform()
        };
    }

    /// <summary>
    /// Gets the current platform name
    /// </summary>
    private static string GetCurrentPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "macOS";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        return "Unknown";
    }

    #endregion
}

/// <summary>
/// Information about the current path configuration
/// </summary>
public class PathInfo
{
    public bool IsUsingCustomPath { get; set; }
    public string CustomPathVariable { get; set; } = string.Empty;
    public string BaseConfigDirectory { get; set; } = string.Empty;
    public string DesktopModuleDirectory { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
}