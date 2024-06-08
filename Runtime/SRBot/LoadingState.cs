using ReactiveUI.Fody.Helpers;

namespace SRBot;

public class LoadingState(bool loading = false, string message = "")
{
    [Reactive] public bool IsLoading { get; set; } = loading;
    [Reactive] public string Message { get; set; } = message;
}