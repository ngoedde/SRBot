using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SukiUI.Controls;

namespace SRBot.Dialog;

public partial class MessageBoxDialog : UserControl
{
    public MessageBoxDialog()
    {
        InitializeComponent();
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MessageBoxDialogModel model)
            return;

        model.Result = UserConfirmation.Cancel;
        
        SukiHost.CloseDialog();
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MessageBoxDialogModel model)
            return;

        model.Result = UserConfirmation.Ok;
        
        SukiHost.CloseDialog();
    }
}