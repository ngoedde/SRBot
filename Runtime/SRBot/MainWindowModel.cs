using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Media;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRBot.Config;
using SRBot.Dialog;
using SRBot.Page;
using SRCore;
using SRCore.Bot;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRGame.Client;
using SRNetwork;
using SRNetwork.Common;
using SukiUI.Controls;

namespace SRBot;

public class MainWindowModel : ViewModel
{
    private readonly ProfileService _profileService;
    private readonly ConfigService _configService;
    private readonly Game _game;
    private readonly AppConfigLoader _appConfigLoader;
    private readonly ClientInfoManager _clientInfoManager;
    private readonly EntityManager _entityManager;
    private readonly Proxy _proxy;
    private readonly BotManager _botManager;
    private readonly PatchInfo _patchInfo;
    private readonly ServerListDialogModel _serverListDialogModel;

    #region Properties

    /// <summary>
    /// Gets the pages displayed in the main window.
    /// </summary>
    public ObservableCollection<PageModel> Pages { get; }

    /// <summary>
    /// Gets or sets the current title of the application window.
    /// </summary>
    [Reactive]
    public string Title { get; set; } = "SRBot";

    /// <summary>
    /// Gets the current profile that is being used.
    /// </summary>
    public Profile ActiveProfile => _profileService.ActiveProfile;

    /// <summary>
    /// Gets or sets the current loading state of the application.
    /// </summary>
    [Reactive]
    public LoadingState LoadingState { get; set; } = new();

    /// <summary>
    /// A value indicating if the game is initialized.
    /// </summary>
    public bool IsGameInitialized => _game.IsLoaded;

    /// <summary>
    /// Gets a value indicating if the server list is available.
    /// </summary>
    public bool IsServerListAvailable => _proxy.Context == ProxyContext.Gateway;

    #endregion

    public MainWindowModel(IServiceProvider serviceProvider)
    {
        var pages = serviceProvider.GetRequiredService<IEnumerable<PageModel>>();
        Pages = new ObservableCollection<PageModel>(pages.OrderBy(x => x.Position));

        // Get required services from the service provider, to not blow up the constructor.
        _clientInfoManager = serviceProvider.GetRequiredService<ClientInfoManager>();
        _entityManager = serviceProvider.GetRequiredService<EntityManager>();
        _configService = serviceProvider.GetRequiredService<ConfigService>();
        _appConfigLoader = serviceProvider.GetRequiredService<AppConfigLoader>();
        _profileService = serviceProvider.GetRequiredService<ProfileService>();
        _game = serviceProvider.GetRequiredService<Game>();
        _proxy = serviceProvider.GetRequiredService<Proxy>();
        _botManager = serviceProvider.GetRequiredService<BotManager>();
        _patchInfo = serviceProvider.GetRequiredService<PatchInfo>();
        _serverListDialogModel = serviceProvider.GetRequiredService<ServerListDialogModel>();

        _game.GameStopLoading += OnGameStopLoading;
        _game.GameStartLoading += OnGameStartLoading;
        _profileService.ActiveProfileChanged += OnActiveProfileChanged;
        _patchInfo.PatchInfoUpdated += OnPatchInfoUpdated;
    }

    private async void OnPatchInfoUpdated(Session session, PatchInfo patchInfo)
    {
        if (patchInfo.PatchRequired)
        {
            App.MessageBoxManager.ShowMessageBox("Patch required",
                "A new patch is required to play the game. Please restart the game to apply the patch.",
                MessageBoxButtons.Ok, MaterialIconKind.Update, Brushes.Red);
        }
    }

    #region Event listener

    private void OnGameStopLoading(Game kernel)
    {
        SetLoading(new LoadingState());

        this.RaisePropertyChanged(nameof(IsGameInitialized));
    }

    private void OnGameStartLoading(Game kernel)
    {
        SetLoading(new LoadingState(true, "Loading game..."));

        this.RaisePropertyChanged(nameof(IsGameInitialized));
    }

    private async void OnActiveProfileChanged(Profile profile)
    {
        if (!Directory.Exists(_profileService.ActiveProfile.ClientDirectory))
        {
            ShowProfileDialog();
        }

        this.RaisePropertyChanged(nameof(ActiveProfile));

        await _appConfigLoader.LoadConfigAsync(profile);

        var logConfig = _configService.GetConfig<LogConfig>();
        if (logConfig != null)
            App.LoggingLevelSwitch.MinimumLevel = logConfig.LogLevel;

        await _game.CloseAsync();
        await _game.LoadGameDataAsync();
    }

    #endregion

    #region Methods

    public void SetLoading(LoadingState state) => LoadingState = state;

    public async Task SwitchBotState()
    {
        if (_botManager.CurrentBot.State is BotState.Idle or BotState.Stopped)
            await StartBot();
        else
            await StopBot();
    }

    public async Task StartBot()
    {
        // Network has not been started yet?
        if (_proxy.Context == ProxyContext.None)
        {
            var gatewayHost = _clientInfoManager.DivisionInfo.Divisions[0].GatewayServers[0];
            EndPoint gatewayEndPoint = NetHelper.ToIPEndPoint(gatewayHost, _clientInfoManager.GatewayPort);

            if (_profileService.ActiveProfile.Clientless)
            {
                await _proxy.ConnectToGateway(gatewayEndPoint);
            }
            else
            {
                await _proxy.StartClientProxy(_profileService.ActiveProfile.ClientListeningPort);
            }
        }
    }

    public async Task StopBot()
    {
    }

    public async Task SaveConfig()
    {
        await _configService.SaveAllAsync();
        await _profileService.SaveProfilesAsync();
    }

    public void ShowProfileDialog()
    {
        var dialogModel = new ProfileDialogModel(_profileService, _game);

        SukiHost.ShowDialog(dialogModel);
    }

    public void ShowClientInfoDialog()
    {
        var dialog = new ClientInfoDialogModel(_clientInfoManager, _entityManager);

        SukiHost.ShowDialog(dialog, allowBackgroundClose: true);
    }

    public void ShowServerListDialog()
    {
        SukiHost.ShowDialog(_serverListDialogModel, allowBackgroundClose: true);
    }
    
    #endregion
}