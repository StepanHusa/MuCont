using Avalonia.Controls;
using MuCont.Desktop.Dialogs;
using MuCont.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services;
public class DialogService : IDialogService
{
    private readonly MainWindowViewModel mainWM;
    private readonly IServiceProvider serviceProvider;

    public DialogService(MainWindowViewModel mainWM, IServiceProvider serviceProvider)
    {
        this.mainWM = mainWM;
        this.serviceProvider = serviceProvider;
    }

    public async Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>()
        where TView : UserControl, new()
        where TViewModel : class, IDialogViewModel<TResult>
    {
        var viewModel = serviceProvider.GetService(typeof(TViewModel)) as TViewModel;

        if (viewModel == null)
        {
            throw new InvalidOperationException($"Unable to resolve ViewModel of type {typeof(TViewModel).Name}.");
        }

        var view = new TView
        {
            DataContext = viewModel
        };

        mainWM.ShowOverlay(view);

        var tcs = new TaskCompletionSource<TResult?>();

        var overlayViewModel = viewModel as IDialogViewModel<TResult>;

        overlayViewModel.OnClose = result =>
        {
            mainWM.HideOverlay();
            tcs.SetResult(result);
        };

        return await tcs.Task;
    }
}

