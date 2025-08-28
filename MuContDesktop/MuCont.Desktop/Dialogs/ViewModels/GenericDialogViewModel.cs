using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Dialogs.ViewModels;

internal partial class GenericDialogViewModel : ObservableObject, IDialogIOViewModel<GenericDialogConfig, DialogResult<GenericDialogResult>>
{
    public string Title { get; set; } = "Dialog";

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private bool _isOnlyRead = true;

    public Action<DialogResult<GenericDialogResult>?> OnClose { get; set; } = _ => { };

    public void OnOpened(GenericDialogConfig input)
    {
        Title = input.Title ?? "Dialog";
        Message = input.Text ?? string.Empty;
        IsOnlyRead = input.IsOnlyRead;
    }

    [RelayCommand]
    public void OnOk()
    {
        var result = new GenericDialogResult { ClosedOk = true };
        OnClose(DialogResult<GenericDialogResult>.Ok(result));
    }

    [RelayCommand]
    public void OnCancel()
    {
        OnClose(DialogResult<GenericDialogResult>.Cancel());
    }

    [RelayCommand]
    public void OnCloseDialog()
    {
        OnClose(DialogResult<GenericDialogResult>.Cancel());
    }
}

public class GenericDialogConfig
{
    public bool IsOnlyRead { get; set; } = true;
    public string? Text { get; set; } = null;
    public string? Title { get; set; } = null;
}

public class GenericDialogResult
{
    public bool ClosedOk { get; set; }
}