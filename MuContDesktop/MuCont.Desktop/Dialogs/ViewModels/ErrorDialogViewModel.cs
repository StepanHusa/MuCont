using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Dialogs.ViewModels;

public class ErrorDialogData
{
    public string ErrorMessage { get; set; } = string.Empty;
    public bool WasAcknowledged { get; set; }
}

internal partial class ErrorDialogViewModel : ObservableObject, IDialogIOViewModel<string, DialogResult<ErrorDialogData>>
{
    public string Title { get; set; } = "Error";

    [ObservableProperty]
    private string _message = string.Empty;

    public Action<DialogResult<ErrorDialogData>?> OnClose { get; set; } = _ => { };

    public void OnOpened(string input)
    {
        Message = input;
    }

    [RelayCommand]
    public void OnOk()
    {
        var data = new ErrorDialogData
        {
            ErrorMessage = Message,
            WasAcknowledged = true
        };
        OnClose(DialogResult<ErrorDialogData>.Ok(data));
    }

    [RelayCommand]
    public void OnCloseDialog()
    {
        OnClose(DialogResult<ErrorDialogData>.Cancel());
    }
}
