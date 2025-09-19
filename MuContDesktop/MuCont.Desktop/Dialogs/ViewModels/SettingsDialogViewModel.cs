using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MuCont.Desktop.Services;
using TextCopy;


namespace MuCont.Desktop.Dialogs.ViewModels;

public class SettingsData
{
    public string Theme { get; set; } = string.Empty;
    public bool StartMaximized { get; set; }
    public string LastProject { get; set; } = string.Empty;
    public bool UseLocalApi { get; set; }
    public string ApiAddress { get; set; } = string.Empty;
}

public partial class SettingsDialogViewModel : ObservableObject, IDialogViewModel<DialogResult<SettingsData>>
{
    private readonly IOptions<AppSettings> _options;
    private readonly IConfiguration _configuration;
    private readonly IFileService _fileService;

    #region Startup Settings
    [ObservableProperty]
    private string _selectedTheme = string.Empty;

    [ObservableProperty]
    private bool _startMaximized;

    [ObservableProperty]
    private string _lastProject = string.Empty;
    #endregion

    #region API Settings
    [ObservableProperty]
    private bool _useLocalApi = true;

    [ObservableProperty]
    private string _apiAddress = "http://localhost:5000";
    #endregion

    #region Path Information
    [ObservableProperty]
    private string _logFolderPath = string.Empty;

    [ObservableProperty]
    private string _baseConfigDirectory = string.Empty;

    [ObservableProperty]
    private string _desktopModuleDirectory = string.Empty;

    [ObservableProperty]
    private string _settingsDirectory = string.Empty;

    [ObservableProperty]
    private string _cacheDirectory = string.Empty;

    [ObservableProperty]
    private string _tempDirectory = string.Empty;

    [ObservableProperty]
    private string _applicationDirectory = string.Empty;

    [ObservableProperty]
    private PathInfo _pathInfo = new();
    #endregion

    #region UI State
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    [ObservableProperty]
    private int _selectedTabIndex = 0;
    #endregion

    [RelayCommand]
    public async Task OnApplyAsync()
    {
        var success = await TrySaveSettingsAsync();
        if (success)
        {
            HasUnsavedChanges = false;
            // Settings saved successfully, but keep dialog open
            // You could show a success message here if needed
        }
        // If save failed, dialog stays open so user can fix the issue
    }

    [RelayCommand]
    public async Task OnSaveAndCloseAsync()
    {
        var success = await TrySaveSettingsAsync();
        if (success)
        {
            var settingsData = new SettingsData
            {
                Theme = SelectedTheme,
                StartMaximized = StartMaximized,
                LastProject = LastProject,
                UseLocalApi = UseLocalApi,
                ApiAddress = ApiAddress
            };
            OnClose(DialogResult<SettingsData>.Ok(settingsData));
        }
        // If save failed, dialog stays open so user can fix the issue
    }

    private async Task<bool> TrySaveSettingsAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            await SaveSettingsAsync();
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save settings: {ex.Message}";
            return false;
        }
    }

    [RelayCommand]
    public void OnCancel()
    {
        // Close dialog without saving any changes
        OnClose(DialogResult<SettingsData>.Cancel());
    }

    [RelayCommand]
    public void OnCloseDialog()
    {
        // Same as cancel - close without saving
        OnClose(DialogResult<SettingsData>.Cancel());
    }

    #region Path Copy Commands
    [RelayCommand]
    public async Task CopyLogFolderPathAsync()
    {
        await CopyPathToClipboardAsync(LogFolderPath);
    }

    [RelayCommand]
    public async Task CopyBaseConfigDirectoryAsync()
    {
        await CopyPathToClipboardAsync(BaseConfigDirectory);
    }

    [RelayCommand]
    public async Task CopyDesktopModuleDirectoryAsync()
    {
        await CopyPathToClipboardAsync(DesktopModuleDirectory);
    }

    [RelayCommand]
    public async Task CopySettingsDirectoryAsync()
    {
        await CopyPathToClipboardAsync(SettingsDirectory);
    }

    [RelayCommand]
    public async Task CopyCacheDirectoryAsync()
    {
        await CopyPathToClipboardAsync(CacheDirectory);
    }

    [RelayCommand]
    public async Task CopyTempDirectoryAsync()
    {
        await CopyPathToClipboardAsync(TempDirectory);
    }

    [RelayCommand]
    public async Task CopyApplicationDirectoryAsync()
    {
        await CopyPathToClipboardAsync(ApplicationDirectory);
    }
    #endregion

    private async Task CopyPathToClipboardAsync(string path)
    {
        try
        {
            // Use TextCopy for cross-platform clipboard support
            var clipboard = new Clipboard();
            await clipboard.SetTextAsync(path);

            // Clear any previous error messages on successful copy
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to copy path to clipboard: {ex.Message}";
        }
    }

    public SettingsDialogViewModel(IOptions<AppSettings> options, IConfiguration configuration, IFileService fileService)
    {
        _options = options;
        _configuration = configuration;
        _fileService = fileService;

        LoadCurrentSettings();
        InitializePaths();
        SetupPropertyChangeTracking();
    }

    private void LoadCurrentSettings()
    {
        var settings = _options.Value;
        SelectedTheme = settings.Theme;
        LastProject = settings.LastOpenedFile;
        StartMaximized = settings.StartMaximized;
        UseLocalApi = settings.UseLocalApi;
        ApiAddress = settings.ApiAddress;
    }

    private void InitializePaths()
    {
        // Initialize all paths from PathManager
        LogFolderPath = PathManager.GetDesktopLogsDirectory();
        BaseConfigDirectory = PathManager.GetBaseConfigDirectory();
        DesktopModuleDirectory = PathManager.GetDesktopModuleDirectory();
        SettingsDirectory = PathManager.GetDesktopSettingsDirectory();
        CacheDirectory = PathManager.GetDesktopCacheDirectory();
        TempDirectory = PathManager.GetDesktopTempDirectory();
        ApplicationDirectory = PathManager.GetApplicationDirectory();
        PathInfo = PathManager.GetPathInfo();
    }

    private void SetupPropertyChangeTracking()
    {
        // Track property changes to detect unsaved changes
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName != nameof(HasUnsavedChanges) && 
                e.PropertyName != nameof(ErrorMessage) && 
                e.PropertyName != nameof(SelectedTabIndex) &&
                !IsPathProperty(e.PropertyName))
            {
                HasUnsavedChanges = true;
            }
        };
    }

    private bool IsPathProperty(string? propertyName)
    {
        return propertyName switch
        {
            nameof(LogFolderPath) or
            nameof(BaseConfigDirectory) or
            nameof(DesktopModuleDirectory) or
            nameof(SettingsDirectory) or
            nameof(CacheDirectory) or
            nameof(TempDirectory) or
            nameof(ApplicationDirectory) or
            nameof(PathInfo) => true,
            _ => false
        };
    }

    private async Task SaveSettingsAsync()
    {
        // Create the updated settings object
        var updatedSettings = new AppSettings
        {
            Theme = SelectedTheme,
            LastOpenedFile = LastProject,
            StartMaximized = StartMaximized,
            UseLocalApi = UseLocalApi,
            ApiAddress = ApiAddress
        };

        // Save using the FileService which handles proper path management and error handling
        await _fileService.SaveSettingsAsync("app-settings.json", updatedSettings);

        // Also update the configuration in memory for immediate effect
        _configuration["AppSettings:Theme"] = SelectedTheme;
        _configuration["AppSettings:LastOpenedFile"] = LastProject;
        _configuration["AppSettings:StartMaximized"] = StartMaximized.ToString();
        _configuration["AppSettings:UseLocalApi"] = UseLocalApi.ToString();
        _configuration["AppSettings:ApiAddress"] = ApiAddress;
    }

    public Action<DialogResult<SettingsData>?> OnClose { get; set; } = _ => { };
}
