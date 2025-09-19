using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MuCont.Desktop.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private static readonly string ApplicationName = "MuCont";
    private static readonly string DesktopModuleName = "Desktop";

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Path Resolution

    public string GetApplicationDataPath()
    {
        return PathManager.GetBaseConfigDirectory();
    }

    public string GetDesktopModulePath()
    {
        return PathManager.GetDesktopModuleDirectory();
    }

    public string GetSettingsFilePath(string fileName)
    {
        return PathManager.GetDesktopSettingsFilePath(fileName);
    }


    #endregion

    #region Settings Operations

    public async Task<T?> LoadSettingsAsync<T>(string fileName) where T : class
    {
        try
        {
            var filePath = GetSettingsFilePath(fileName);

            if (!await FileExistsAsync(filePath))
            {
                _logger.LogInformation("Settings file {FileName} not found, returning null", fileName);
                return null;
            }

            var json = await ReadTextAsync(filePath);
            var settings = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            _logger.LogInformation("Successfully loaded settings from {FileName}", fileName);
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings from {FileName}", fileName);
            throw;
        }
    }

    public async Task SaveSettingsAsync<T>(string fileName, T settings) where T : class
    {
        try
        {
            var filePath = GetSettingsFilePath(fileName);

            // Ensure directory exists using PathManager
            var directory = Path.GetDirectoryName(filePath)!;
            PathManager.EnsureDirectoryExists(directory);

            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            await WriteTextAsync(filePath, json);

            _logger.LogInformation("Successfully saved settings to {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings to {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> SettingsFileExistsAsync(string fileName)
    {
        var filePath = GetSettingsFilePath(fileName);
        return await FileExistsAsync(filePath);
    }

    public async Task DeleteSettingsAsync(string fileName)
    {
        var filePath = GetSettingsFilePath(fileName);
        await DeleteFileAsync(filePath);
    }

    #endregion

    #region General File Operations

    public async Task<string> ReadTextAsync(string filePath)
    {
        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read text from {FilePath}", filePath);
            throw;
        }
    }

    public async Task WriteTextAsync(string filePath, string content)
    {
        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                await CreateDirectoryAsync(directory);
            }

            await File.WriteAllTextAsync(filePath, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write text to {FilePath}", filePath);
            throw;
        }
    }

    public async Task<byte[]> ReadBytesAsync(string filePath)
    {
        try
        {
            return await File.ReadAllBytesAsync(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read bytes from {FilePath}", filePath);
            throw;
        }
    }

    public async Task WriteBytesAsync(string filePath, byte[] content)
    {
        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                await CreateDirectoryAsync(directory);
            }

            await File.WriteAllBytesAsync(filePath, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write bytes to {FilePath}", filePath);
            throw;
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        return Task.FromResult(File.Exists(filePath));
    }

    public Task<bool> DirectoryExistsAsync(string directoryPath)
    {
        return Task.FromResult(Directory.Exists(directoryPath));
    }

    public async Task CreateDirectoryAsync(string directoryPath)
    {
        try
        {
            if (!await DirectoryExistsAsync(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                _logger.LogDebug("Created directory {DirectoryPath}", directoryPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create directory {DirectoryPath}", directoryPath);
            throw;
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        try
        {
            if (await FileExistsAsync(filePath))
            {
                File.Delete(filePath);
                _logger.LogDebug("Deleted file {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FilePath}", filePath);
            throw;
        }
    }

    public async Task DeleteDirectoryAsync(string directoryPath, bool recursive = false)
    {
        try
        {
            if (await DirectoryExistsAsync(directoryPath))
            {
                Directory.Delete(directoryPath, recursive);
                _logger.LogDebug("Deleted directory {DirectoryPath} (recursive: {Recursive})", directoryPath, recursive);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete directory {DirectoryPath}", directoryPath);
            throw;
        }
    }

    public async Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
    {
        try
        {
            // Ensure destination directory exists
            var destinationDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDir))
            {
                await CreateDirectoryAsync(destinationDir);
            }

            File.Copy(sourcePath, destinationPath, overwrite);
            _logger.LogDebug("Copied file from {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy file from {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
            throw;
        }
    }

    public async Task MoveFileAsync(string sourcePath, string destinationPath)
    {
        try
        {
            // Ensure destination directory exists
            var destinationDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDir))
            {
                await CreateDirectoryAsync(destinationDir);
            }

            File.Move(sourcePath, destinationPath);
            _logger.LogDebug("Moved file from {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to move file from {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
            throw;
        }
    }

    #endregion

    #region File System Information

    public Task<FileInfo> GetFileInfoAsync(string filePath)
    {
        return Task.FromResult(new FileInfo(filePath));
    }

    public Task<DirectoryInfo> GetDirectoryInfoAsync(string directoryPath)
    {
        return Task.FromResult(new DirectoryInfo(directoryPath));
    }

    public async Task<IEnumerable<string>> GetFilesAsync(string directoryPath, string searchPattern = "*", bool recursive = false)
    {
        try
        {
            if (!await DirectoryExistsAsync(directoryPath))
            {
                return Enumerable.Empty<string>();
            }

            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.GetFiles(directoryPath, searchPattern, searchOption);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get files from {DirectoryPath}", directoryPath);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetDirectoriesAsync(string directoryPath, string searchPattern = "*", bool recursive = false)
    {
        try
        {
            if (!await DirectoryExistsAsync(directoryPath))
            {
                return Enumerable.Empty<string>();
            }

            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.GetDirectories(directoryPath, searchPattern, searchOption);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get directories from {DirectoryPath}", directoryPath);
            throw;
        }
    }

    #endregion

    #region Recent Files Management

    public async Task<List<string>> GetRecentFilesAsync()
    {
        try
        {
            var recentFiles = await LoadSettingsAsync<List<string>>("recent-files.json");
            return recentFiles ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task AddRecentFileAsync(string filePath)
    {
        try
        {
            var recentFiles = await GetRecentFilesAsync();

            // Remove if already exists to move to top
            recentFiles.Remove(filePath);

            // Add to beginning
            recentFiles.Insert(0, filePath);

            // Keep only last 10 files
            if (recentFiles.Count > 10)
            {
                recentFiles = recentFiles.Take(10).ToList();
            }

            await SaveSettingsAsync("recent-files.json", recentFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add recent file {FilePath}", filePath);
        }
    }

    public async Task RemoveRecentFileAsync(string filePath)
    {
        try
        {
            var recentFiles = await GetRecentFilesAsync();
            recentFiles.Remove(filePath);
            await SaveSettingsAsync("recent-files.json", recentFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove recent file {FilePath}", filePath);
        }
    }

    public async Task ClearRecentFilesAsync()
    {
        try
        {
            await SaveSettingsAsync("recent-files.json", new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear recent files");
        }
    }

    #endregion

    #region Backup and Restore

    public async Task<string> CreateBackupAsync(string filePath)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupPath = $"{filePath}.backup_{timestamp}";

            await CopyFileAsync(filePath, backupPath);
            _logger.LogInformation("Created backup of {FilePath} at {BackupPath}", filePath, backupPath);

            return backupPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create backup of {FilePath}", filePath);
            throw;
        }
    }

    public async Task RestoreFromBackupAsync(string backupPath, string targetPath)
    {
        try
        {
            await CopyFileAsync(backupPath, targetPath, overwrite: true);
            _logger.LogInformation("Restored {TargetPath} from backup {BackupPath}", targetPath, backupPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore {TargetPath} from backup {BackupPath}", targetPath, backupPath);
            throw;
        }
    }

    #endregion

    #region File Watching

    public FileSystemWatcher CreateFileWatcher(string path, string filter = "*.*")
    {
        return new FileSystemWatcher(path, filter);
    }

    #endregion

    #region Utility Methods

    public string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Concat(fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return sanitized;
    }

    public string GetFileExtension(string filePath)
    {
        return Path.GetExtension(filePath);
    }

    public string GetFileNameWithoutExtension(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }

    public long GetFileSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

    public DateTime GetLastWriteTime(string filePath)
    {
        return File.GetLastWriteTime(filePath);
    }

    #endregion
}
