using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services;
public class DialogService :IDialogService
{
    private readonly Window _mainWindow;
    private readonly IServiceProvider serviceProvider;

    public DialogService(MainWindow mainWindow, IServiceProvider serviceProvider)
    {
        _mainWindow = mainWindow;
        this.serviceProvider = serviceProvider;
    }

    public async Task<TResult?> ShowDialogAsync<TView, TViewModel, TResult>()
        where TView : UserControl, new()
        where TViewModel : class
    {
        var viewModel = serviceProvider.GetService(typeof(TViewModel)) as TViewModel;

        var dialogWindow = new Window
        {
            Width = 400,
            Height = 300,
            Content = new TView
            {
                DataContext = viewModel
            }
        };

        return await dialogWindow.ShowDialog<TResult?>(_mainWindow);
    }
}

