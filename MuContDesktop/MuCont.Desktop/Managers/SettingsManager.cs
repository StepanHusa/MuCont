using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuCont.Desktop.Managers;
public static class SettingsManager
{
    public static void SaveSettings(AppSettings settings)
    {
        var directory = Path.GetDirectoryName(AppPaths.SettingsFilePath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AppPaths.SettingsFilePath, json);
    }

    public static AppSettings LoadSettings()
    {
        if (File.Exists(AppPaths.SettingsFilePath))
        {
            string json = File.ReadAllText(AppPaths.SettingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        return new AppSettings(); // Default settings
    }

}