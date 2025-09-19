using System.Threading.Tasks;

namespace MuCont.Desktop.Services;

public interface IFileService
{
    // Path Resolution
    string GetApplicationDataPath();
    string GetDesktopModulePath();
    string GetSettingsFilePath(string fileName);

    // Settings Operations
    Task<T?> LoadSettingsAsync<T>(string fileName) where T : class;
    Task SaveSettingsAsync<T>(string fileName, T settings) where T : class;
    Task<bool> SettingsFileExistsAsync(string fileName);
}
