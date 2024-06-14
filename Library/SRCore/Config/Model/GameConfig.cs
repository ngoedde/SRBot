using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Config.Model;

public class GameConfig : ConfigElement
{
    private bool _enableAutoLogin = false;
    public const string FileName = "game.json";

    public bool EnableAutoLogin
    {
        get => _enableAutoLogin;
        set
        {
            this.RaiseAndSetIfChanged(ref _enableAutoLogin, value);

            if (!_enableAutoLogin)
                Clientless = false;
        }
    }

    [Reactive] public int AutoLoginId { get; set; } = 0;
    [Reactive] public int AutoLoginDelay { get; set; } = 0;
    [Reactive] public string AutoLoginServer { get; set; } = string.Empty;
    [Reactive] public string AutoLoginCharacter { get; set; } = string.Empty;
    [Reactive] public string StaticCaptcha { get; set; } = string.Empty;
    [Reactive] public bool Clientless { get; set; }

    public int GetAutoLoginDelayTime()
    {
        var secondsToWait = AutoLoginDelay == 0 ? new Random().Next(1000, 3000) : AutoLoginDelay * 1000;

        return secondsToWait;
    }
}