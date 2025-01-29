using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using Dock.Model.Core;
using Microsoft.Extensions.DependencyInjection;
using MuCont.ComputationInterface;
using MuCont.Desktop.DockingUtilities;
using MuCont.Desktop.Services;
using MuCont.Desktop.ViewModels.Dockable;
using MuCont.Desktop.ViewModels.Dockable.Plots;
using MuCont.Desktop.Views;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels;
internal partial class MainViewModel : ObservableObject
{
    private readonly IDockFactory? _factory;
    private readonly IEventAggregator ea;
    private readonly IDialogService dialogService;
    private IRequestManager _requestManager;
    private IRootDock? _layout;

    public IRootDock? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }


    public MainViewModel(IEventAggregator ea, IDialogService dialogService)
    {
        _factory = new DockFactory(new DockingContext());
        _requestManager = new RequestManager(ea, "D:\\Systems", "julia");


        //DebugFactoryEvents(_factory);

        Layout = _factory?.CreateLayout();
        if (Layout is { })
        {
            _factory?.InitLayout(Layout);
            if (Layout is { } root)
            {
                root.Navigate.Execute("Home");
            }
        }

        this.ea = ea;
        this.dialogService = dialogService;
    }

    [RelayCommand]
    private async Task OnOpenSettingsAsync()
    {
        await dialogService.ShowDialogAsync<SettingsView, SettingsViewModel, object>();
    }

    [RelayCommand]
    private void OnNewSystem()
    {
        _requestManager.RunTestTaskOnJulia();
    }

    [RelayCommand]
    private void OnWindowOutput()
    {
        var dockable = new JuliaOutputViewModel(ea) { Id = $"Output", Title = $"Output" };

        _factory?.AddDockableToCentralDock(dockable);
    }

    public void CloseLayout()
    {
        if (Layout is IDock dock)
        {
            if (dock.Close.CanExecute(null))
            {
                dock.Close.Execute(null);
            }
        }
    }

    public void ResetLayout()
    {
        if (Layout is not null)
        {
            if (Layout.Close.CanExecute(null))
            {
                Layout.Close.Execute(null);
            }
        }

        var layout = _factory?.CreateLayout();
        if (layout is not null)
        {
            Layout = layout;
            _factory?.InitLayout(layout);
        }
    }


}
