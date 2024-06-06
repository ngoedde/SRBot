using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;

namespace SRBot.Dialog;

public partial class ClientInfoDialog : UserControl
{
    public ClientInfoDialog()
    {
        InitializeComponent();
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SukiHost.CloseDialog();
    }
}