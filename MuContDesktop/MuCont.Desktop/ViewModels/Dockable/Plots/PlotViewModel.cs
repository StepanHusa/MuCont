using Dock.Model.Mvvm.Controls;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.ViewModels.Dockable.Plots;
internal class PlotViewModel : Tool
{
    public PlotModel PlotModel { get; set; }
    public ObservableCollection<string> AvailableCurves { get; set; } = new();
    public ObservableCollection<string> SelectedCurves { get; set; } = new();

    public PlotViewModel()
    {
        PlotModel = new PlotModel { Title = "Curves" };

        // Initialize sample data
        InitializeSampleData();

        // Refresh plot when selected curves change
        SelectedCurves.CollectionChanged += (s, e) => UpdatePlot();
    }

    private void InitializeSampleData()
    {
        // Example data
        AvailableCurves.Add("Curve 1");
        AvailableCurves.Add("Curve 2");
        AvailableCurves.Add("Curve 3");

        SelectedCurves.Add("Curve 1"); // Default selected curve
        UpdatePlot();
    }

    private void UpdatePlot()
    {
        PlotModel.Series.Clear();

        // Add only selected curves
        foreach (var curve in SelectedCurves)
        {
            if (curve == "Curve 1")
                PlotModel.Series.Add(CreateLineSeries("Curve 1", x => x, x => x));
            if (curve == "Curve 2")
                PlotModel.Series.Add(CreateLineSeries("Curve 2", x => x, x => x * x));
            if (curve == "Curve 3")
                PlotModel.Series.Add(CreateLineSeries("Curve 3", x => x, x => x * x * x));
        }

        PlotModel.InvalidatePlot(true); // Refresh plot
    }

    private LineSeries CreateLineSeries(string title, Func<double, double> xFunc, Func<double, double> yFunc)
    {
        var series = new LineSeries { Title = title };

        for (double x = -10; x <= 10; x += 0.1)
        {
            series.Points.Add(new DataPoint(xFunc(x), yFunc(x)));
        }

        return series;
    }
}