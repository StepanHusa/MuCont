using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MuCont.Desktop.Dialogs;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Text.Json;


namespace MuCont.Desktop.ViewModels;


public partial class SettingsViewModel : ObservableObject, IDialogViewModel<bool>
{
    private readonly IOptions<AppSettings> _options;
    private readonly IConfiguration _configuration;

    public string SelectedTheme { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }

    [RelayCommand]
    public void OnSave()
    {
        SaveSettings();
        OnClose(true);
    }

    [RelayCommand] 
    public void OnCancel()
    {
        OnClose(false);
    }

    public SettingsViewModel(IOptions<AppSettings> options, IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;

        var settings = _options.Value;
        SelectedTheme = settings.Theme;
        WindowWidth = settings.WindowWidth;
        WindowHeight = settings.WindowHeight;
    }

    private void SaveSettings()
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
    }

    public Action<bool> OnClose { get; set; } = _ => { };

}
