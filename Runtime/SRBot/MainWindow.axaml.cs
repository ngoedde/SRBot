using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using SRBot.Dialog;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
using SRGame.Client;
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

        if (string.IsNullOrEmpty(_profileService.Config.ActiveProfile))
        {
            OpenProfileDialog();
        }
        else
        {
            await _profileService.SetActiveProfileAsync(_profileService.Config.ActiveProfile);
            await _kernel.InitializeGameAsync(_configService.GetConfig<GameConfig>()!.ClientDirectory,
                ClientType.Vietnam188);
        }
    }

    private void OpenProfileDialog()
    {
        var dialogModel = new ProfileDialogModel(_profileService);

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
        if (!_kernel.IsNetworkInitialized)
        {
            await _kernel.StartNetworkAsync();
        }
    }
}