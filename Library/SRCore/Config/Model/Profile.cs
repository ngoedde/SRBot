using System.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using SRCore.Utils;
using SRGame.Client;

namespace SRCore.Config.Model;

public class Profile(string configDirectory, string name) : ConfigElement
{
    [Required, Reactive] public string Name { get; set; } = name;

    [Reactive, DirectoryExists] public string ConfigDirectory { get; set; } = configDirectory;
    [Reactive] public string Description { get; set; } = string.Empty;

    [Reactive] public ushort ClientListeningPort { get; set; } = 16000;

    [Required, DirectoryExists, Reactive] public string ClientDirectory { get; set; } = string.Empty;

    [Reactive] public string PackPassword { get; set; } = "169841";

    [Reactive] public ClientType ClientType { get; set; } = ClientType.Vietnam188;

    public static implicit operator string(Profile profile) => profile.Name;

    public override string ToString()
    {
        return Name;
    }
}