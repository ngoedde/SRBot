using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRBot.Page;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRBot.Plugin.Game.Page;

public class GamePageModel : PageModel
{
    private readonly ProfileService _profileService;
    private readonly ConfigService _configService;

    public Profile? ActiveProfile => _profileService.ActiveProfile;

    public IEnumerable<string> ProfileNames => _profileService.Config.Profiles.Select(p => p.Name);

    [Reactive] public GameConfig? GameConfig => _configService.GetConfig<GameConfig>();

    public GamePageModel(ProfileService profileService, ConfigService configService) : base("srbot_page_game", "Game",
        0, MaterialIconKind.Gamepad)
    {
        _profileService = profileService;
        _configService = configService;
        _profileService.ProfileConfigLoaded += ProfileServiceOnProfileConfigLoaded;
        _profileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;
    }

    private void ProfileServiceOnActiveProfileChanged(Profile profile)
    {
        this.RaisePropertyChanged(nameof(ActiveProfile));
        this.RaisePropertyChanged(nameof(GameConfig));
    }

    private void ProfileServiceOnProfileConfigLoaded(ProfileConfig config)
    {
        this.RaisePropertyChanged(nameof(ProfileNames));
    }
}