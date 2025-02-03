using Avalonia.Controls;
using MuCont.Desktop.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services;
public interface IDialogService
{
    Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>()
        where TView : UserControl, new()
        where TViewModel : class, IDialogViewModel<TResult>;
}
