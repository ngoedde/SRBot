using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class ProfileConfig : ReactiveObject
{
     public const string DefaultProfileName = "default";
     [Reactive] public string ActiveProfile { get; set; } = DefaultProfileName;
     [Reactive] public List<Profile> Profiles { get; set; } = new();
     
     public Profile GetActiveProfile() => Profiles.First(p => p.Name == ActiveProfile || p.Name == DefaultProfileName);
}