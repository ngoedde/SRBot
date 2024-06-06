using System.ComponentModel.DataAnnotations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Utils;

namespace SRCore.Config.Model;

public class Profile(string configDirectory, string name) : ReactiveObject
{

    [Required]
    [Reactive] public string Name { get; set; } = name;

    [Reactive] public string ConfigDirectory { get; set; } = configDirectory;
    [Reactive] public string Description { get; set; } = string.Empty;
    [Reactive] public bool Clientless { get; set; } = true;
    [Reactive] public ushort ClientlessPort { get; set; } = 16000;

    [Required]
    [DirectoryExists]
    [Reactive]
    public string ClientDirectory { get; set; } = string.Empty;

    [Reactive] public string PackPassword { get; set; } = "169841";
}