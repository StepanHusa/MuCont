using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace MuCont.Desktop.Dialogs.ViewModels;

public class SettingsData
{
    public string Theme { get; set; } = string.Empty;
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
}

public partial class SettingsDialogViewModel : ObservableObject, IDialogViewModel<DialogResult<SettingsData>>
{
    private readonly IOptions<AppSettings> _options;
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private string _selectedTheme = string.Empty;

    [ObservableProperty]
    private int _windowWidth;

    [ObservableProperty]
    private int _windowHeight;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

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
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight
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

    public SettingsDialogViewModel(IOptions<AppSettings> options, IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;

        var settings = _options.Value;
        SelectedTheme = settings.Theme;
        WindowWidth = settings.WindowWidth;
        WindowHeight = settings.WindowHeight;

        // Track property changes to detect unsaved changes
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName != nameof(HasUnsavedChanges) && e.PropertyName != nameof(ErrorMessage))
            {
                HasUnsavedChanges = true;
            }
        };
    }

    private async Task SaveSettingsAsync()
    {
        await Task.Run(() =>
        {
            // Update settings
            _configuration["AppSettings:Theme"] = SelectedTheme;
            _configuration["AppSettings:WindowWidth"] = WindowWidth.ToString();
            _configuration["AppSettings:WindowHeight"] = WindowHeight.ToString();

            // Save updated configuration to file
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var updatedSettings = new AppSettings
            {
                Theme = SelectedTheme,
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight
            };
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(updatedSettings, new JsonSerializerOptions { WriteIndented = true }));
        });
    }

    public Action<DialogResult<SettingsData>?> OnClose { get; set; } = _ => { };

}
