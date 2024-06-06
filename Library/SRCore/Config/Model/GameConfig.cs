using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class GameConfig : ConfigElement
{
    public const string FileName = "game.json";

    [Reactive] public string ClientDirectory { get; set; } = string.Empty;

    [Reactive] public string PackPassword { get; set; } = "169841";
}