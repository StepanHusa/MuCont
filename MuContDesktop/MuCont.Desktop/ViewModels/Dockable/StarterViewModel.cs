using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels.Dockable;
internal partial class StarterViewModel : Tool
{
    public ObservableCollection<InputFieldViewModel> Coordinates { get; } = new();
    public ObservableCollection<InputFieldViewModel> Parameters { get; } = new();

    public void LoadSystem(DynamicalSystem system)
    {
        Coordinates.Clear();
        foreach (var name in system.Coordinates)
            Coordinates.Add(new InputFieldViewModel { Name = name });

        Parameters.Clear();
        foreach (var name in system.Parameters)
            Parameters.Add(new InputFieldViewModel { Name = name });
    }



}
public class DynamicalSystem
{
    public List<string> Coordinates { get; set; } = new();
    public List<string> Parameters { get; set; } = new();
}


    public partial class InputFieldViewModel : ObservableObject
{
    public required string Name { get; init; }

    [ObservableProperty]
    private double value;

}
