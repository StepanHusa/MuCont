using Dock.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.DockingUtilities;
internal interface IDockFactory : IFactory
{
    void AddDockableToCentralDock(IDockable dockable);
}
