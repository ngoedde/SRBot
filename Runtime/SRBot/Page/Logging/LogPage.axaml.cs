using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicData.Binding;
using SukiUI.Controls;
using SukiUI.Enums;

namespace SRBot.Page.Logging;

public partial class LogPage : UserControl
{
    private LogPageModel? ViewModel => this.DataContext as LogPageModel;

    public LogPage()
    {
        InitializeComponent();
    }

    private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!ViewModel?.ScrollToEnd == true)
            return;

        var log = ViewModel?.Logs.LastOrDefault();
        if (log == null)
            return;

        LogGrid.ScrollIntoView(log, LogGrid.Columns[0]);
    }

    private void ClearLog_OnClick(object? sender, RoutedEventArgs e)
    {
        var model = this.DataContext! as LogPageModel;
        model!.ClearLogs();

        SukiHost.ShowToast("Log cleared", "Log has been cleared!", SukiToastType.Success);
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        ViewModel.Logs.CollectionChanged += Logs_CollectionChanged;
    }
}