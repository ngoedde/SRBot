using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class GameConfig : ConfigElement
{
    public const string FileName = "game.json";

    [Reactive] public bool EnableAutoLogin { get; set; } = false;
    [Reactive] public int AutoLoginId { get; set; } = 0;
    [Reactive] public string AutoLoginServer { get; set; } = string.Empty;
    [Reactive] public string AutoLoginCharacter { get; set; } = string.Empty;

    [Reactive] public string StaticCaptcha { get; set; } = string.Empty;
}