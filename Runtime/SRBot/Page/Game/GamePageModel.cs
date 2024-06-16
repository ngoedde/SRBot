using Material.Icons;
using ReactiveUI;
using SRBot.Dialog.GamePage;
using SRCore.Components;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Service;
using SukiUI.Controls;
using ViewLocator = SRBot.Utils.ViewLocator;

namespace SRBot.Page.Game;

public class GamePageModel(AccountService accountService, ViewLocator viewLocator, ConfigService configService)
    : PageModel("srbot_page_game",
        "Game",
        0, MaterialIconKind.Gamepad)
{
    private int _autoLoginId;

    public AutoLoginConfig AutoLoginConfig => accountService.Config;
    public GameConfig GameConfig => configService.GetConfig<GameConfig>() ?? new GameConfig();

    public AccountInfo? SelectedAutoLoginAccount
    {
        get => accountService.GetAccount(GameConfig.AutoLoginId);
        set
        {
            GameConfig.AutoLoginId = value?.Id ?? 0;

            this.RaiseAndSetIfChanged(ref _autoLoginId, value?.Id ?? 0);
        }
    }

    public void OpenAccountManagerDialog()
    {
        var vm = new AccountManagerDialogModel(accountService);

        var dialog = viewLocator.Build(vm) as AccountManagerDialog;
        SukiHost.ShowDialog(dialog, allowBackgroundClose: true);
    }
}