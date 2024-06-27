using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ScottPlot;
using ScottPlot.Avalonia;
using SRCore;
using SukiUI.Controls;

namespace SRBot.Windows;

public partial class PerformanceWindow : SukiWindow
{
    private PerformanceWindowModel Model => DataContext as PerformanceWindowModel;
    private List<double> frameTimes = new List<double>();
    private List<double> timeStamps = new List<double>();
    private Stopwatch stopwatch = new Stopwatch();
    private AvaPlot plot;
    
    
    public PerformanceWindow()
    {
        InitializeComponent();
        stopwatch.Start();
        
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var kernel = Model.Kernel;
        
        if (e.PropertyName == nameof(KernelMetrics.FrameTime))
        {
            frameTimes.Add(kernel.Metrics.FrameTime);
            timeStamps.Add(stopwatch.Elapsed.TotalSeconds);

            plot.Plot.Clear();
            plot.Plot.Add.ScatterLine(timeStamps.ToArray(), frameTimes.ToArray());
            plot.Refresh();
        }
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        plot = this.FindControl<AvaPlot>("PerformancePlot");
        Model.Kernel.Metrics.PropertyChanged += ModelOnPropertyChanged;
    }
}