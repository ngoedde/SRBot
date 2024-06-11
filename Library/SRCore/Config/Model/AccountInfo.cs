using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class AccountInfo(int id = 0) : ReactiveObject
{
    [Reactive] public int Id { get; set; } = id;
    [Reactive] public string Username { get; set; } = string.Empty;
    [Reactive] public string Password { get; set; } = string.Empty;
    [Reactive] public string SecondaryPassword { get; set; } = string.Empty;
}