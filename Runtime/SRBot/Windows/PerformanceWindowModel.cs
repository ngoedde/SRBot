using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ScottPlot;
using SRCore;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRGame.Client.Repository;
using ViewLocator = SRBot.Utils.ViewLocator;

namespace SRBot.Windows;

public class PerformanceWindowModel(Kernel kernel) : ViewModel
{
    public Kernel Kernel { get; } = kernel;

}