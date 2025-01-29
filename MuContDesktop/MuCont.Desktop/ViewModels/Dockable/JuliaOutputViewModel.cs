using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using Prism.Events;
using System;
using System.Threading.Tasks;
using MuCont.ComputationInterface.Events;
using System.Security.AccessControl;

namespace MuCont.Desktop.ViewModels.Dockable;
internal partial class JuliaOutputViewModel : Tool
{
    private readonly IEventAggregator _ea;
    [ObservableProperty]
    private string _output = String.Empty;

    [ObservableProperty]
    private bool _isRunning = false;

    [ObservableProperty]
    private string _statusMessage = "";

    public JuliaOutputViewModel(IEventAggregator ea)
    {
        _ea = ea;

        _ea.GetEvent<NewOutputEvent>().Subscribe(OnNewOutput);

        //TODO there should be also output that came before the window opened
    }

    private void OnNewOutput(string? newLines)
    {
        Output += newLines;
    }


}
