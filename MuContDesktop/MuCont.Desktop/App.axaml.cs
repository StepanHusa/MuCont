using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MuCont.Desktop.Themes;
using MuCont.Desktop.ViewModels;
using MuCont.Desktop.Views;
using System;

namespace MuCont.Desktop
{
    public partial class App : Application
    {
        public static IThemeManager? ThemeManager;

        private IServiceProvider _serviceProvider;

        public override void Initialize()
        {
            ThemeManager = new FluentThemeManager();
            ThemeManager.Initialize(this);

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // DockManager.s_enableSplitToWindow = true;

 

            switch (ApplicationLifetime)
            {
                case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                    {
                        // Set up DI
                        _serviceProvider = Startup.ConfigureServices();

                        var mainWindowViewModel = _serviceProvider.GetService<MainWindowViewModel>();
                        var mainViewModel = _serviceProvider.GetService<MainViewModel>(); // the singleton of VeiwModel Containing possible undocked windows


                        if (mainWindowViewModel is null)
                        {
                            throw new InvalidOperationException("MainWindowViewModel is not registered in the service provider");
                        }

                        if (mainViewModel is null)
                        {
                            throw new InvalidOperationException("MainViewModel is not registered in the service provider");
                        }

                        mainWindowViewModel.MainViewModelProp = mainViewModel; //This is here because mainViewModel depends on dialogService which back depends on mainWindowViewModel

                        var mainWindow = new MainWindow
                        {
                            DataContext = mainWindowViewModel
                        };

                        


                        mainWindow.Closing += (_, _) =>
                        {

                                mainViewModel.CloseLayout();

                        };

                        desktopLifetime.MainWindow = mainWindow;

                        desktopLifetime.Exit += (_, _) =>
                        {

                                mainViewModel.CloseLayout();

                        };

                        break;
                    }
                //case ISingleViewApplicationLifetime singleViewLifetime:
                //    {
                //        var mainView = new MainView()
                //        {
                //            DataContext = mainWindowViewModel
                //        };

                //        singleViewLifetime.MainView = mainView;

                //        break;
                //    }
            }

            base.OnFrameworkInitializationCompleted();
#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}