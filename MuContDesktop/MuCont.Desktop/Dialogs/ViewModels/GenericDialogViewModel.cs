using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace MuCont.Desktop.Dialogs.ViewModels;
internal partial class GenericDialogViewModel : ObservableObject, IDialogIOViewModel<GenericDialogConfig, GenericDialogResult>
{
    public string Title { get; set; } = "Error";

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private bool _isOnlyRead = true;

    Action<GenericDialogResult?> IDialogIOViewModel<GenericDialogConfig, GenericDialogResult>.OnClose { get; set; } = _ => OnClose();  // this is not correct 


    public void OnOpened(GenericDialogConfig input)
    {
    
        Title = input.Title ?? string.Empty;
        Message = input.Text ?? string.Empty;
        IsOnlyRead = input.IsOnlyRead;
    }

    public static GenericDialogResult? OnClose()
    {
        return new GenericDialogResult
        {
            ClosedOk = true,
        };
    }
}

internal class GenericDialogConfig
{
    public bool IsOnlyRead { get; set; } = true;
    public string? Text { get; set; } = null;
    public string? Title { get; set; } = null;
}

internal class GenericDialogResult
{
    public bool ClosedOk { get; set; }
}