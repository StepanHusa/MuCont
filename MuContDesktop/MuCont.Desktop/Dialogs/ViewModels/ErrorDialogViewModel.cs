using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Dialogs.ViewModels;
internal partial class ErrorDialogViewModel : ObservableObject, IDialogIOViewModel<string, bool>
{
    public string Title { get; set; } = "Error";

    [ObservableProperty]
    private string _message = string.Empty;

    public Action<bool> OnClose { get; set; } = _ => { };

    public void OnOpened(string input)
    {
        Message = input;
    }
}
