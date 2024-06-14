using System.Threading.Tasks;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Material.Icons;
using SRBot.Dialog;
using SukiUI.Controls;

namespace SRBot.Utils;

public class MessageBoxManager(IDataTemplate viewLocator)
{
    public void ShowMessageBox(string title, string message, MessageBoxButtons buttons = MessageBoxButtons.None,
        MaterialIconKind icon = MaterialIconKind.Information, IImmutableSolidColorBrush iconColor = null)
    {
        var model = new MessageBoxDialogModel
        {
            Title = title,
            Message = message,
            Buttons = buttons,
            Icon = icon,
            IconColor = iconColor
        };

        var msgBox = CreateFromUIThread(model);

        Dispatcher.UIThread.Invoke(() => SukiHost.ShowDialog(msgBox));
    }

    public async Task<MessageBoxDialogResult> ShowMessageBoxAsync(string title, string message,
        MessageBoxButtons buttons = MessageBoxButtons.None, MaterialIconKind icon = MaterialIconKind.Information,
        IImmutableSolidColorBrush iconColor = null)
    {
        var model = new MessageBoxDialogModel
        {
            Title = title,
            Message = message,
            Buttons = buttons,
            Icon = icon,
            IconColor = iconColor
        };

        var msgBox = CreateFromUIThread(model);

        Dispatcher.UIThread.Invoke(() => { SukiHost.ShowDialog(msgBox); });

        while (model.Result == MessageBoxDialogResult.None)
        {
            await Task.Delay(10);
        }

        return model.Result;
    }

    private MessageBoxDialog CreateFromUIThread(MessageBoxDialogModel model)
    {
        var msgBox = Dispatcher.UIThread.Invoke(() =>
        {
            var messageBox = viewLocator.Build(model) as MessageBoxDialog;

            return messageBox;
        });

        return msgBox;
    }
}