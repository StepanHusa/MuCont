using System.Threading.Tasks;

namespace MuCont.Desktop.Services;

public interface IApplicationRestartService
{
    /// <summary>
    /// Restarts the application
    /// </summary>
    Task RestartApplicationAsync();
    
    /// <summary>
    /// Safely shuts down the application and prepares for restart
    /// </summary>
    Task PrepareForRestartAsync();
}