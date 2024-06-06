using System.Threading.Tasks;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI.Fody.Helpers;
using SukiUI;
using SukiUI.Models;

namespace SRBot.Dialog;

public class MessageBoxDialogModel
{
    [Reactive] public string Title { get; set; } = "Message title";
    [Reactive] public string Message { get; set; } = "Message content";
    [Reactive] public MaterialIconKind Icon { get; set; } = MaterialIconKind.Information;
    [Reactive] public UserConfirmation Result { get; set; } = UserConfirmation.None;
    
    [Reactive] public IImmutableSolidColorBrush IconColor { get; set; } = Brushes.Black;
    
    public async Task<UserConfirmation> WaitForResultAsync()
    {
        while (Result == UserConfirmation.None)
        {
            await Task.Delay(100);
        }

        return Result;
    }
}