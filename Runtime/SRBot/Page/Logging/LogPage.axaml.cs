using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;
using SukiUI.Enums;

namespace SRBot.Page.Logging;

public partial class LogPage : UserControl
{
    public LogPage()
    {
        InitializeComponent();
    }

    private void ClearLog_OnClick(object? sender, RoutedEventArgs e)
    {
        var model = this.DataContext! as LogPageModel;
        model!.ClearLogs();

        SukiHost.ShowToast("Log cleared", "Log has been cleared!", SukiToastType.Success);
    }
}