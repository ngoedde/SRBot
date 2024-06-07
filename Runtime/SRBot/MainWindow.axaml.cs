using System.IO;
using System.Net;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using SRBot.Dialog;
using SRCore;
using SRCore.Config;
using SRGame.Client;
using SRNetwork.Common;
using SukiUI.Controls;
using SukiUI.Enums;

namespace SRBot;

public partial class MainWindow : SukiWindow
{
    private readonly ProfileService _profileService = App.ServiceProvider.GetRequiredService<ProfileService>();
    private readonly ConfigService _configService = App.ServiceProvider.GetRequiredService<ConfigService>();
    private readonly EntityManager _entityManager = App.ServiceProvider.GetRequiredService<EntityManager>();
    private readonly ClientInfoManager _clientInfoManager = App.ServiceProvider.GetRequiredService<ClientInfoManager>();
    private readonly Kernel _kernel = App.ServiceProvider.GetRequiredService<Kernel>();
    private readonly Game _game = App.ServiceProvider.GetRequiredService<Game>();
    private readonly Proxy _proxy = App.ServiceProvider.GetRequiredService<Proxy>();

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (!_kernel.IsInitialized)
        {
            await SukiHost.ShowToast("Kernel error", "Kernel is not initialized!", SukiToastType.Error);

            return;
        }
        
        if (!Directory.Exists(_profileService.ActiveProfile.ClientDirectory))
        {
            OpenProfileDialog();
        
            return;
        }
        
        await _game.LoadGameDataAsync();
    }

    private void OpenProfileDialog()
    {
        var dialogModel = new ProfileDialogModel(_profileService, _kernel);

        SukiHost.ShowDialog(dialogModel);
    }

    private void ProfileName_OnClick(object? sender, RoutedEventArgs e)
    {
        OpenProfileDialog();
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await _configService.SaveAllAsync();
        await _profileService.SaveProfilesAsync();
    }

    private void ShowClientInfo_OnClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new ClientInfoDialogModel(_clientInfoManager, _entityManager);

        SukiHost.ShowDialog(dialog, allowBackgroundClose: true);
    }

    private async void StartButton_OnClick(object? sender, RoutedEventArgs e)
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
}