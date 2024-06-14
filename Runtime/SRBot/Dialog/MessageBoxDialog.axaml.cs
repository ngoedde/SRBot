using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;

namespace SRBot.Dialog;

public partial class MessageBoxDialog : UserControl
{
    private MessageBoxDialogModel ViewModel => DataContext as MessageBoxDialogModel ?? new MessageBoxDialogModel();

    public MessageBoxDialog()
    {
        InitializeComponent();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Result = MessageBoxDialogResult.Cancel;

        SukiHost.CloseDialog();
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Result = MessageBoxDialogResult.Ok;

        SukiHost.CloseDialog();
    }

    private void RetryButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Result = MessageBoxDialogResult.Retry;

        SukiHost.CloseDialog();
    }

    private void YesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Result = MessageBoxDialogResult.Ok;

        SukiHost.CloseDialog();
    }

    private void NoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Result = MessageBoxDialogResult.Cancel;

        SukiHost.CloseDialog();
    }
}