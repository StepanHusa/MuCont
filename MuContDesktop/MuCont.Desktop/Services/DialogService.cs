using Avalonia.Controls;
using MuCont.Desktop.Dialogs;
using MuCont.Desktop.Dialogs.ViewModels;
using MuCont.Desktop.Dialogs.Views;
using MuCont.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services;
internal class DialogService : IDialogService
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
        // TODO make a native way to close the dialog
        return await tcs.Task;
    }

    public async Task<TResult?> ShowIODialogAsync<TView, TViewModel, TInput, TResult>(TInput input)
    where TView : UserControl, new()
    where TViewModel : class, IDialogIOViewModel<TInput,TResult>
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

        var overlayViewModel = viewModel as IDialogIOViewModel<TInput, TResult>;

        overlayViewModel.OnClose = result =>
        {
            mainWM.HideOverlay();
            tcs.SetResult(result);
        };
        overlayViewModel.OnOpened(input); 

        return await tcs.Task;
    }

    public async Task ShowErrorDialog(string errorMessage)
    {
        await ShowIODialogAsync<ErrorDialogView,ErrorDialogViewModel,string,bool>(errorMessage);
    }

    public async Task ShowGenericDialogAsync(GenericDialogConfig config)
    {
        await ShowIODialogAsync<GenericDialogView, GenericDialogViewModel, GenericDialogConfig, GenericDialogResult>(config);
    }
}

