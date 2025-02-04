using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using Microsoft.Extensions.DependencyInjection;
using MuCont.Desktop.ViewModels;
using MuCont.Desktop.ViewModels.Dockable;
using MuCont.Desktop.ViewModels.Dockable.Plots;
using System;
using System.Collections.Generic;

namespace MuCont.Desktop.DockingUtilities;

internal class DockFactory : Factory, IDockFactory
{
    private readonly DockingContext _context;
    private readonly IServiceProvider serviceProvider;
    private IRootDock? _rootDock;
    private IDock? _centralDock;

    public DockFactory(DockingContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        this.serviceProvider = serviceProvider;
    }

    //public override IDocumentDock CreateDocumentDock() => new CustomDocumentDock();

    public override IRootDock CreateLayout()
    {
        var plot1 = serviceProvider.GetRequiredService<PlotViewModel>();
        plot1.Id = "Plot1";
        plot1.Title = "Plot1";
        plot1.CanClose = true;

        var plot2 = serviceProvider.GetRequiredService<PlotViewModel>();
        plot2.Id = "Plot2";
        plot2.Title = "Plot2";
        plot2.CanClose = true;

        var starter = serviceProvider.GetRequiredService<StarterViewModel>();
        starter.Id = "Starter";
        starter.Title = "Starter";

        var systems = serviceProvider.GetRequiredService<SystemsViewModel>();
        systems.Id = "Systems";
        systems.Title = "Systems";


        var leftDock = new ProportionalDock
        {
            Proportion = 0.25,
            Orientation = Orientation.Vertical,
            ActiveDockable = systems,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = starter,
                    VisibleDockables = CreateList<IDockable>(systems,starter),
                    Alignment = Alignment.Left
                },
                new ProportionalDockSplitter()
            )
        };



        var centralDock = new CentralDockViewModel
        {
            IsCollapsable = false,
            ActiveDockable = plot1,
            VisibleDockables = CreateList<IDockable>( plot1,plot2),            
        };

        var mainLayout = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>
            (
                leftDock,
                new ProportionalDockSplitter(),
                centralDock

            )
        };

        var dashboardView = new DashboardViewModel
        {
            Id = "Dashboard",
            Title = "Dashboard"
        };

        var homeView = new HomeViewModel
        {
            Id = "Home",
            Title = "Home",
            ActiveDockable = mainLayout,
            VisibleDockables = CreateList<IDockable>(mainLayout)
        };

        var rootDock = CreateRootDock();

        rootDock.IsCollapsable = false;
        rootDock.ActiveDockable = dashboardView;
        rootDock.DefaultDockable = homeView;
        rootDock.VisibleDockables = CreateList<IDockable>(dashboardView, homeView);

        _centralDock = centralDock;
        _rootDock = rootDock;

        return rootDock;
    }

    public override IDockWindow? CreateWindowFrom(IDockable dockable)
    {
        var window = base.CreateWindowFrom(dockable);

        if (window != null)
        {
            window.Title = "MuCont v100";
        }
        return window;
    }

    public override void InitLayout(IDockable layout)
    {
        // This seems pretty unimportant but maybe when something stops working, setting this up will solve the problem.

        //ContextLocator = new Dictionary<string, Func<object?>>
        //{
        //    ["Document1"] = () => new DemoDocument(),
        //    ["Document2"] = () => new DemoDocument(),
        //    ["Document3"] = () => new DemoDocument(),
        //    ["Tool1"] = () => new Tool1(),
        //    ["Tool2"] = () => new Tool2(),
        //    ["Plot1"] = () => new PlotTool(),
        //    ["Dashboard"] = () => layout,
        //    ["Home"] = () => _context
        //};


        DockableLocator = new Dictionary<string, Func<IDockable?>>()
        {
            ["Root"] = () => _rootDock,
            ["Documents"] = () => _centralDock
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }


    public void AddDockableToCentralDock(IDockable dockable)
    {
        if (_centralDock is null)
            return;

        this.AddDockable(_centralDock, dockable);
        this.SetActiveDockable(dockable);
        this.SetFocusedDockable(_centralDock, dockable);
    }

}
