using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using MuCont.Desktop.Services.ApiServices.SystemsService;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels.Dockable.Plots;
internal partial class PlotViewModel : Tool
{
    public PlotModel PlotModel { get; set; }
    [ObservableProperty]
    private ObservableCollection<SelectableCurve> availableCurves = new();

    public IEnumerable<SelectableCurve> SelectedCurves => AvailableCurves
        .Where(sc => sc.IsSelected);

    public PlotViewModel()
    {
        PlotModel = new PlotModel { Title = "Curves" };

        // Initialize sample data
        InitializeSampleData();

        // Refresh plot when selected curves change
        //SelectedCurves.CollectionChanged += (s, e) => UpdatePlot()
    }

    private void InitializeSampleData()
    {
        // Example data
        AvailableCurves.Add(new SelectableCurve { Id = "Curve 1" , IsSelected = true});
        AvailableCurves.Add(new SelectableCurve { Id = "Curve 2" , IsSelected = true });
        AvailableCurves.Add(new SelectableCurve { Id = "Curve 3" });

        //SelectedCurves.Add("Curve 1"); // Default selected curve
        UpdatePlot();
    }

    [RelayCommand]
    private void OnRedraw()
    {
        UpdatePlot();
    }

    private void UpdatePlot()
    {
        PlotModel.Series.Clear();

        // Add only selected curves
        foreach (var curve in SelectedCurves)
        {
            if (curve.Id == "Curve 1")
                PlotModel.Series.Add(CreateLineSeries("Curve 1", x => x, x => x));
            if (curve.Id == "Curve 2")
                PlotModel.Series.Add(CreateLineSeries("Curve 2", x => x, x => x * x));
            if (curve.Id == "Curve 3")
                PlotModel.Series.Add(CreateLineSeries("Curve 3", x => x, x => x * x * x));
        }

        PlotModel.InvalidatePlot(true); // Refresh plot
    }

    private LineSeries CreateLineSeries(string title, Func<double, double> xFunc, Func<double, double> yFunc)
    {
        var series = new LineSeries { Title = title };

        for (double x = -1; x <= 1; x += 0.1)
        {
            series.Points.Add(new DataPoint(xFunc(x), yFunc(x)));
        }

        return series;
    }
}

internal partial class SelectableCurve : ObservableObject
{
    public string Id { get; init; }

    [ObservableProperty]
    private bool isSelected;
}
