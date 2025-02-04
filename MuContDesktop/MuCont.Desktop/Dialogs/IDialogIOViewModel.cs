using System;

namespace MuCont.Desktop.Dialogs;

internal interface IDialogIOViewModel<TInput,TResult>
{
    Action<TResult?> OnClose { get; set; }
    public void OnOpened(TInput input);
}