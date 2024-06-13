using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using SRCore.Botting;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRGame.Client;
using SRNetwork;
using SukiUI.Controls;

namespace SRBot;

public class MainWindowModel : ViewModel
{
    private readonly IServiceProvider _serviceProvider;
    public Bot Bot => _serviceProvider.GetRequiredService<Bot>();
    public ProfileService ProfileService => _serviceProvider.GetRequiredService<ProfileService>();
    public ConfigService ConfigService => _serviceProvider.GetRequiredService<ConfigService>();
    public Game Game => _serviceProvider.GetRequiredService<Game>();
    public AppConfigLoader AppConfigLoader => _serviceProvider.GetRequiredService<AppConfigLoader>();
    public Proxy Proxy => _serviceProvider.GetRequiredService<Proxy>();
    public PatchInfo PatchInfo => _serviceProvider.GetRequiredService<PatchInfo>();
    public ServerListDialogModel ServerListDialogModel => _serviceProvider.GetRequiredService<ServerListDialogModel>();
    public ClientInfoManager ClientInfoManager => _serviceProvider.GetRequiredService<ClientInfoManager>();
    public EntityManager EntityManager => _serviceProvider.GetRequiredService<EntityManager>();

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
    public Profile ActiveProfile => ProfileService.ActiveProfile;

    /// <summary>
    /// Gets or sets the current loading state of the application.
    /// </summary>
    [Reactive]
    public LoadingState LoadingState { get; set; } = new();

    /// <summary>
    /// A value indicating if the game is initialized.
    /// </summary>
    public bool IsGameInitialized => Game.IsLoaded;

    /// <summary>
    /// Gets a value indicating if the server list is available.
    /// </summary>
    public bool IsServerListAvailable => (Proxy.Context & ProxyContext.Gateway) != 0;

    public bool IsBotRunning => Bot.CurrentBot?.State is BotState.Started;
    public bool IsBotIdle => Bot.CurrentBot?.State is BotState.Idle;

    public Player Player => _serviceProvider.GetRequiredService<Player>();
  
    #endregion

    public MainWindowModel(IServiceProvider serviceProvider)
    {
        var pages = serviceProvider.GetRequiredService<IEnumerable<PageModel>>();
        Pages = new ObservableCollection<PageModel>(pages.OrderBy(x => x.Position));

        // Get required services from the service provider, to not blow up the constructor.
        _serviceProvider = serviceProvider;
       

        Game.GameStopLoading += OnGameStopLoading;
        Game.GameStartLoading += OnGameStartLoading;
        ProfileService.ActiveProfileChanged += OnActiveProfileChanged;
        PatchInfo.PatchInfoUpdated += OnPatchInfoUpdated;
        Bot.BotStarted += OnBotStarted;
        Bot.BotStopped += OnBotStopped;
    }

    private void OnBotStopped(BotBase bot)
    {
        this.RaisePropertyChanged(nameof(IsBotRunning));
        this.RaisePropertyChanged(nameof(IsBotIdle));
    }

    private void OnBotStarted(BotBase bot)
    {
        this.RaisePropertyChanged(nameof(IsBotRunning));
        this.RaisePropertyChanged(nameof(IsBotIdle));
    }

    private async void OnPatchInfoUpdated(Session session, PatchInfo patchInfo)
    {
        if (patchInfo.PatchRequired)
        {
            App.MessageBoxManager.ShowMessageBox("Patch required",
                "A new patch is required to play the game. Please restart the game to apply the patch.",
                MessageBoxButtons.Ok, MaterialIconKind.Update, Brushes.Red);
        }

        this.RaisePropertyChanged(nameof(IsServerListAvailable));
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
        if (!Directory.Exists(ProfileService.ActiveProfile.ClientDirectory))
        {
            ShowProfileDialog();
        }

        this.RaisePropertyChanged(nameof(ActiveProfile));

        await AppConfigLoader.LoadConfigAsync(profile);

        var logConfig = ConfigService.GetConfig<LogConfig>();
        if (logConfig != null)
            App.LoggingLevelSwitch.MinimumLevel = logConfig.LogLevel;

        await Game.CloseAsync();
        await Game.LoadGameDataAsync();
    }

    #endregion

    #region Methods

    public void SetLoading(LoadingState state) => LoadingState = state;

    public async Task SwitchBotState()
    {
        if (Bot.State is BotState.Idle)
            await StartBot();
        else
            await StopBot();
    }

    public async Task StartBot()
    {
        // Network has not been started yet?
        if (Proxy.Context == ProxyContext.None)
        {
            var gatewayEndPoint = ClientInfoManager.GetGatewayEndPoint();

            if (ProfileService.ActiveProfile.Clientless)
            {
                await Proxy.ConnectToGateway(gatewayEndPoint);
            }
            else
            {
                await Proxy.StartClientProxy(ProfileService.ActiveProfile.ClientListeningPort);
            }
        }
        
        Bot.StartBot();
    }

    public async Task StopBot()
    {
        Bot.StopBot();
    }

    public async Task SaveConfig()
    {
        // await _configService.SaveAllAsync();
        // await _profileService.SaveProfilesAsync();
    }

    public void ShowProfileDialog()
    {
        var dialogModel = new ProfileDialogModel(ProfileService, Game);

        SukiHost.ShowDialog(dialogModel);
    }

    public void ShowClientInfoDialog()
    {
        var dialog = new ClientInfoDialogModel(ClientInfoManager, EntityManager);

        SukiHost.ShowDialog(dialog, allowBackgroundClose: true);
    }

    public void ShowServerListDialog()
    {
        SukiHost.ShowDialog(ServerListDialogModel, allowBackgroundClose: true);
    }
    
    #endregion
}