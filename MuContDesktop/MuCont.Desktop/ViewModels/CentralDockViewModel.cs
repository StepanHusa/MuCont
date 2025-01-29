using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using Dock.Model.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuCont.Desktop.ViewModels.Dockable.Plots;
using System.Runtime.Serialization;
using Dock.Model.Controls;
using Dock.Model.Mvvm.Core;

namespace MuCont.Desktop.ViewModels;

[DataContract(IsReference = true)]
public partial class CentralDockViewModel : DocumentDock
{
    // TODO rewrite this as a CentralDockViewModel :  DockBase and make a veiw for it (find out how to do it)
    public CentralDockViewModel()
    {
        CreateDocument = new RelayCommand(CreateNewDocument);
    }

    private void CreateNewDocument()
    {
        if (!CanCreateDocument)
        {
            return;
        }

        var index = VisibleDockables?.Count + 1;
        var document = new PlotViewModel { Id = $"Plot{index}", Title = $"Plot{index}" };

        Factory?.AddDockable(this, document);
        Factory?.SetActiveDockable(document);
        Factory?.SetFocusedDockable(this, document);
    }
}
