using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels;
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentOverlayView;

    [ObservableProperty]
    private bool _isOverlayVisible;

    [ObservableProperty]
    private MainViewModel _mainViewModelProp;


    public void ShowOverlay(UserControl content)
    {
        
        CurrentOverlayView = content;
        IsOverlayVisible = true;
    }

    public void HideOverlay()
    {
        IsOverlayVisible = false;
        CurrentOverlayView = null;
    }
}
