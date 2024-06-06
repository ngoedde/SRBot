using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class Profile(string configDirectory, string name) : ReactiveObject
{
    [Reactive] public string Name { get; set; } = name;

    [Reactive] public string ConfigDirectory { get; set; } = configDirectory;
    [Reactive] public string Description { get; set; } = string.Empty;
    [Reactive] public bool Clientless { get; set; } = true;
    [Reactive] public ushort ClientlessPort { get; set; } = 16000;
}