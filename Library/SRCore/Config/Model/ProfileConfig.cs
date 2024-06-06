using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class ProfileConfig : ReactiveObject
{
     [Reactive] public string ActiveProfile { get; set; } = string.Empty;
     [Reactive] public List<Profile> Profiles { get; set; } = new();
}