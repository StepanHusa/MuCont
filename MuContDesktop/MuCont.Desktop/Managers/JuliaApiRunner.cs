using MuCont.ComputationInterface.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Managers;
internal class JuliaApiRunner(string scriptPath)
{
    private Process? _juliaProcess;

    private readonly string _scriptPath = scriptPath;

    private readonly IEventAggregator _ea;



    public void Start()
    {
        if (_juliaProcess != null)
        {
            Console.WriteLine("Julia API is already running.");
            return;
        }

        _juliaProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "julia",
                Arguments = _scriptPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _juliaProcess.ErrorDataReceived += (sender, args) => Console.WriteLine($"Julia Error: {args.Data}");
        _juliaProcess.OutputDataReceived += (sender, e) =>
        {
            _ea.GetEvent<NewOutputEvent>().Publish(e.Data);
            // TODO log it here as well would be nice
        };



        _juliaProcess.Start();
        _juliaProcess.BeginOutputReadLine();
        _juliaProcess.BeginErrorReadLine();

        Console.WriteLine("Julia API started.");
    }

    public void Stop()
    {
        if (_juliaProcess != null && !_juliaProcess.HasExited)
        {
            _juliaProcess.Kill();
            _juliaProcess.Dispose();
            _juliaProcess = null;
            Console.WriteLine("Julia API stopped.");
        }
    }

    public bool IsRunning => _juliaProcess != null && !_juliaProcess.HasExited;
}