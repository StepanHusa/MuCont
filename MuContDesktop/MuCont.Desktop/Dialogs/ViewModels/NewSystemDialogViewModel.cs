using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MuCont.Desktop.Services.ApiServices.SystemsService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using MuCont.Desktop.FormsGenerator;
using System.Threading.Tasks;

namespace MuCont.Desktop.Dialogs.ViewModels;

public class NewSystemData
{
    public NewSystemPost SystemPost { get; set; } = new();
    public bool WasCreated { get; set; }
}

internal partial class NewSystemDialogViewModel(ISystemService systemService) : ObservableObject, IDialogViewModel<DialogResult<NewSystemData>>
{
    public Action<DialogResult<NewSystemData>?> OnClose { get; set; } = _ => { };

    [ObservableProperty]
    private NewSystemPost _newSystem = new NewSystemPost();
    private readonly ISystemService systemService = systemService;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [RelayCommand]
    public async Task OnSaveAsync()
    {
            var result = await systemService.PostNewService(NewSystem);
            if (result is null)
            {
                ErrorMessage = "Could not save the system.";
                // OnClose(DialogResult<NewSystemData>.Error("Could not save the system."));
                return;
            }

            var systemData = new NewSystemData
            {
                SystemPost = NewSystem, //TODO remove, we should return an object that is really in database
                WasCreated = true
            };
            OnClose(DialogResult<NewSystemData>.Ok(systemData));

    }

    [RelayCommand]
    public void OnCancel()
    {
        OnClose(DialogResult<NewSystemData>.Cancel());
    }

    [RelayCommand]
    public void OnCloseDialog()
    {
        OnClose(DialogResult<NewSystemData>.Cancel());
    }

}

public partial class NewSystemPost : ObservableObject
{
    [ObservableProperty]
    [property: FormField("Full Name")]
    private string _name = string.Empty;

    [ObservableProperty]
    [property: FormField("Notes")]
    private ObservableCollection<string> _notes = new() { "hello", "note2" };



    [ObservableProperty]
    [property: FormField("Coordinates e.g. x, y, mass, tau, x1")]
    private string _coordinates = string.Empty;

    [ObservableProperty]
    [property: FormField("Parameters e.g. a, b, tau")]
    private string _parameters = string.Empty;

    [ObservableProperty]
    [property: FormField("Equations e.g. x' = a*")]
    private string _equations = string.Empty;

    [ObservableProperty]
    [property: FormField("Time")]
    private string _timeSymbol = "t";

}