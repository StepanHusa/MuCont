using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using MuCont.Desktop.Services;
using MuCont.Desktop.Services.ApiServices.SystemsService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels.Dockable;
internal partial class SystemsViewModel(ISystemService systemService, IDialogService dialogService) : Tool
{
    private readonly ISystemService systemService = systemService;
    private readonly IDialogService dialogService = dialogService;
    [ObservableProperty]
    private ObservableCollection<SystemModel> _systems = new();

    [RelayCommand]
    private Task OnRefreshAsync()
        => LoadSystems();

    private async Task LoadSystems()
    {
        var systems  = await systemService.GetSystems();


        if (systems == null)
        {

        await dialogService.ShowErrorDialog("Could not load systems from api");
            return;
        }

        Systems = new(systems);
    }
}
