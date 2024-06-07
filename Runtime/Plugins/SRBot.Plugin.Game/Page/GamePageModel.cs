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


    [Reactive] public Profile ActiveProfile => _profileService.ActiveProfile;


    public GamePageModel(ProfileService profileService) : base("srbot_page_game", "Game",
        0, MaterialIconKind.Gamepad)
    {
        _profileService = profileService;
        _profileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;
    }

    private void ProfileServiceOnActiveProfileChanged(Profile profile)
    {
        this.RaisePropertyChanged(nameof(ActiveProfile));
        this.RaisePropertyChanged(nameof(GameConfig));
    }
}