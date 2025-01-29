using MuCont.ComputationInterface.Events;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace MuCont.ComputationInterface;

public class RequestManager : IRequestManager
{
    private readonly string rootFolder;

    private readonly string _juliaExecutable;

    private Process? _juliaProcess;

    private readonly IEventAggregator _ea;


    public RequestManager(IEventAggregator ea,string rootFolder,string juliaExecutable)
    {
        _juliaExecutable = juliaExecutable;
        _ea = ea;
        this.rootFolder = rootFolder;
        //if (!Directory.Exists(rootFolder))
        //    throw new DirectoryNotFoundException($"The root folder {rootFolder} does not exist.");



    }
    

    public void RunTestTaskOnJulia()
    {
        int arg1 = 5;
        double arg2 = 10.5;

        _juliaProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _juliaExecutable,
                Arguments = "D:\\task.jl",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        _juliaProcess.Start();

        _juliaProcess.OutputDataReceived += (sender, e) =>
        {
            _ea.GetEvent<NewOutputEvent>().Publish(e.Data);
            // TODO log it here as well would be nice
        };

        _juliaProcess.BeginOutputReadLine();
    }


    // Get a list of all systems
    public List<string> GetSystems()
    {
        var systems = new List<string>();
        foreach (var dir in Directory.GetDirectories(rootFolder))
        {
            systems.Add(Path.GetFileName(dir));
        }
        return systems;
    }


    // Create a new system
    public void CreateSystem(string systemName, string description = "")
    {
        var systemPath = Path.Combine(rootFolder, systemName);

        if (Directory.Exists(systemPath))
            throw new Exception($"System '{systemName}' already exists.");

        Directory.CreateDirectory(systemPath);
        File.WriteAllText(Path.Combine(systemPath, "info.json"), JsonSerializer.Serialize(new
        {
            Name = systemName,
            Description = description,
            CreatedDate = DateTime.Now
        }));
        File.Create(Path.Combine(systemPath, "equations.txt")).Dispose();
        Directory.CreateDirectory(Path.Combine(systemPath, "Diagrams"));
        Directory.CreateDirectory(Path.Combine(systemPath, "Logs"));
    }

    // Delete a system
    public void DeleteSystem(string systemName)
    {
        var systemPath = Path.Combine(rootFolder, systemName);
        if (!Directory.Exists(systemPath))
            throw new DirectoryNotFoundException($"System '{systemName}' not found.");

        Directory.Delete(systemPath, true);
    }

    // Get metadata of a system
    public string GetSystemInfo(string systemName)
    {
        var infoPath = Path.Combine(rootFolder, systemName, "info.json");
        if (!File.Exists(infoPath))
            throw new FileNotFoundException($"Metadata for system '{systemName}' not found.");

        return File.ReadAllText(infoPath);
    }

    // Update metadata of a system
    public void UpdateSystemInfo(string systemName, string description)
    {
        var infoPath = Path.Combine(rootFolder, systemName, "info.json");
        if (!File.Exists(infoPath))
            throw new FileNotFoundException($"Metadata for system '{systemName}' not found.");

        var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(infoPath));
        metadata["Description"] = description;
        metadata["LastUpdated"] = DateTime.Now;

        File.WriteAllText(infoPath, JsonSerializer.Serialize(metadata));
    }

    // Update equations of a system
    public void UpdateEquations(string systemName, string equations)
    {
        var equationsPath = Path.Combine(rootFolder, systemName, "equations.txt");
        if (!File.Exists(equationsPath))
            throw new FileNotFoundException($"Equations file for system '{systemName}' not found.");

        File.WriteAllText(equationsPath, equations);
    }

    // Add a new diagram to a system
    public void AddDiagram(string systemName, string diagramName, string data)
    {
        var diagramsPath = Path.Combine(rootFolder, systemName, "Diagrams");
        if (!Directory.Exists(diagramsPath))
            throw new DirectoryNotFoundException($"System '{systemName}' not found.");

        var diagramPath = Path.Combine(diagramsPath, diagramName);
        if (Directory.Exists(diagramPath))
            throw new Exception($"Diagram '{diagramName}' already exists.");

        Directory.CreateDirectory(diagramPath);
        File.WriteAllText(Path.Combine(diagramPath, "data.csv"), data);
    }

    // Rename a diagram in a system
    public void RenameDiagram(string systemName, string oldName, string newName)
    {
        var diagramsPath = Path.Combine(rootFolder, systemName, "Diagrams");
        var oldPath = Path.Combine(diagramsPath, oldName);
        var newPath = Path.Combine(diagramsPath, newName);

        if (!Directory.Exists(oldPath))
            throw new DirectoryNotFoundException($"Diagram '{oldName}' not found in system '{systemName}'.");

        if (Directory.Exists(newPath))
            throw new Exception($"Diagram '{newName}' already exists.");

        Directory.Move(oldPath, newPath);
    }

    // Delete a diagram from a system
    public void DeleteDiagram(string systemName, string diagramName)
    {
        var diagramPath = Path.Combine(rootFolder, systemName, "Diagrams", diagramName);
        if (!Directory.Exists(diagramPath))
            throw new DirectoryNotFoundException($"Diagram '{diagramName}' not found in system '{systemName}'.");

        Directory.Delete(diagramPath, true);
    }
}
