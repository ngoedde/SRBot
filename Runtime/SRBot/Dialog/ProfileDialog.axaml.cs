using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
using SRGame;
using SRGame.Client;
using SukiUI.Controls;

namespace SRBot.Dialog;

public partial class ProfileDialog : UserControl
{
    public ProfileDialog()
    {
        InitializeComponent();
        
        if (DataContext! is not ProfileDialogModel model)
            return;

        if (string.IsNullOrEmpty(model.SelectedProfile) && model.Profiles.Count > 0)
        {
            model.SelectedProfile = model.Profiles[0].Name;
        }
    }

    private async void LoadProfileButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext! is not ProfileDialogModel model)
            return;
        
        await model.SetActiveProfile(model.SelectedProfile);
        await model.SaveProfiles();
        
        SukiHost.CloseDialog();
        
        var kernel = App.ServiceProvider.GetRequiredService<Kernel>();
        if (kernel.IsInitialized)
            await kernel.ShutdownAsync();
        
        var configService = App.ServiceProvider.GetRequiredService<ConfigService>();
        
        await kernel.InitializeAsync();
        await kernel.InitializeGameAsync(configService.GetConfig<GameConfig>()!.ClientDirectory, ClientType.Vietnam188);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        SukiHost.CloseDialog();
    }
}