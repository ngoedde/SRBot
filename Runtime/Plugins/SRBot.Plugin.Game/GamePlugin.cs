using Material.Icons;

namespace SRBot.Plugin.Game;

public class GamePlugin : AppPlugin
{
    public GamePlugin()
    {
        TechnicalName = typeof(GamePlugin).FullName ?? DisplayName;
        DisplayName = "Game";
        Icon = MaterialIconKind.ErrorOutline;
    }
}