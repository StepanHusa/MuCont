using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using Dock.Model.Core;
using MuCont.ComputationInterface;
using MuCont.Desktop.Dialogs.ViewModels;
using MuCont.Desktop.Dialogs.Views;
using MuCont.Desktop.DockingUtilities;
using MuCont.Desktop.Services;
using MuCont.Desktop.ViewModels.Dockable;
using MuCont.Desktop.Views;
using Prism.Events;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels;
internal partial class MainViewModel : ObservableObject
{
    private readonly IDockFactory? dockFactory;
    private readonly IEventAggregator ea;
    private readonly IDialogService dialogService;
    private IRequestManager _requestManager;
    private IRootDock? _layout;

    public IRootDock? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }


    public MainViewModel(IEventAggregator ea, IDialogService dialogService, IDockFactory dockFactory)
    {
        
        _requestManager = new RequestManager(ea, "D:\\Systems", "julia");


        //DebugFactoryEvents(_factory);

        this.ea = ea;
        this.dialogService = dialogService;
        this.dockFactory = dockFactory;
    }

    public void LoadDockLayout()
    {

        Layout = dockFactory?.CreateLayout();
        if (Layout is { })
        {
            dockFactory?.InitLayout(Layout);
            if (Layout is { } root)
            {
                root.Navigate.Execute("Home");
            }
        }


    }

    [RelayCommand]
    private async Task OnOpenSettingsAsync()
    {
        await dialogService.ShowDialogAsync<SettingsDialogView, SettingsDialogViewModel, bool>();
    }



    [RelayCommand]
    private async Task OnNewSystemAsync()
    {
        await dialogService.ShowDialogAsync<NewSystemDialogView, NewSystemDialogViewModel, bool>();

        _requestManager.RunTestTaskOnJulia();
    }

    [RelayCommand]
    private void OnWindowOutput()
    {
        var dockable = new JuliaOutputViewModel(ea) { Id = $"Output", Title = $"Output" };

        dockFactory?.AddDockableToCentralDock(dockable);
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

        var layout = dockFactory?.CreateLayout();
        if (layout is not null)
        {
            Layout = layout;
            dockFactory?.InitLayout(layout);
        }
    }


}
