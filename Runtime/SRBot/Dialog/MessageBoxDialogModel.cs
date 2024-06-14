using System.Threading.Tasks;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRBot.Dialog;

public class MessageBoxDialogModel : ReactiveObject
{
    private MessageBoxButtons _buttons = MessageBoxButtons.None;

    public string Title { get; set; } = "Message title";
    public string Message { get; set; } = "Message content";
    public MaterialIconKind Icon { get; set; } = MaterialIconKind.Information;
    public MessageBoxDialogResult Result { get; set; } = MessageBoxDialogResult.None;

    public IImmutableSolidColorBrush IconColor { get; set; } = Brushes.Black;

    // public MessageBoxButtons Buttons
    // {
    //     get => _buttons;
    //     init {
    //         this.RaiseAndSetIfChanged(ref _buttons, value);
    //         
    //         this.RaisePropertyChanged(nameof(IsCancelButtonVisible));
    //         this.RaisePropertyChanged(nameof(IsRetryButtonVisible));
    //         this.RaisePropertyChanged(nameof(IsNoButtonVisible));
    //         this.RaisePropertyChanged(nameof(IsYesButtonVisible));
    //         this.RaisePropertyChanged(nameof(IsOkButtonVisible));
    //     }
    // }

    public MessageBoxButtons Buttons { get; init; } = MessageBoxButtons.None;

    public bool IsCancelButtonVisible => (Buttons & MessageBoxButtons.Cancel) == MessageBoxButtons.Cancel;
    public bool IsRetryButtonVisible => (Buttons & MessageBoxButtons.Retry) == MessageBoxButtons.Retry;
    public bool IsNoButtonVisible => (Buttons & MessageBoxButtons.No) == MessageBoxButtons.No;
    public bool IsYesButtonVisible => (Buttons & MessageBoxButtons.Yes) == MessageBoxButtons.Yes;
    public bool IsOkButtonVisible => (Buttons & MessageBoxButtons.Ok) == MessageBoxButtons.Ok;
}