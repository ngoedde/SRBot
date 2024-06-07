using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
using SukiUI.Controls;
using SukiUI.Enums;

namespace SRBot.Dialog;

public partial class ProfileDialog : GlassCard
{
    private ProfileDialogModel? ViewModel => DataContext as ProfileDialogModel;
    
    public ProfileDialog()
    {
        InitializeComponent();
    }

    private async void LoadProfileButtonClicked(object? sender, RoutedEventArgs e)
    {
        SukiHost.CloseDialog();

        if (ViewModel == null)
            return;
        
        var kernel = App.ServiceProvider.GetRequiredService<Kernel>();
        var game = App.ServiceProvider.GetRequiredService<Game>();
        
        if (game.IsLoaded)
        {
            var msgBoxDialogModel = new MessageBoxDialogModel
            {
                Icon = MaterialIconKind.MessageWarning,
                IconColor = Brushes.Gold,
                Title = "The game is already initialized.",
                Message =
                    "The game is already initialized. If you choose a different profile, the bot will reload all data. Do you want to continue?",
            };

            SukiHost.ShowDialog(msgBoxDialogModel);

            if (await msgBoxDialogModel.WaitForResultAsync() != UserConfirmation.Ok)
                return;
        }
        
        //Re-Initialize
        await ViewModel.SetActiveProfile(ViewModel.SelectedProfile);
        await ViewModel.SaveProfiles();

        await kernel.ShutdownAsync();
        await kernel.InitializeAsync();
        await game.LoadGameDataAsync();
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        SukiHost.CloseDialog();
    }

    private void NewProfileButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext! is not ProfileDialogModel model)
            return;

        model.AddProfile();
    }

    private async void BrowseButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
            return;
        
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = App.GetTopLevel();
        if (topLevel == null) {
            await SukiHost.ShowToast("Error", "Could not initialize file browser dialog.");
            
            return;
        }
        
        var selectedFolders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select the Silkroad Online game directory",
        });

        var selectedFolder = selectedFolders.FirstOrDefault()?.Path.AbsolutePath;
        if (selectedFolder == null)
            return;
        
        ViewModel.SelectedProfile.ClientDirectory = selectedFolder;
    }
    
    private async void DeleteProfileButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext! is not ProfileDialogModel model)
            return;

        var activeProfile = App.ServiceProvider.GetRequiredService<Profile>();
        if (model.SelectedProfile == activeProfile)
        {
            await SukiHost.ShowToast("Error", "You can't delete the active profile.", SukiToastType.Error);
            
            return;
        }
        
        var msgBoxDialogModel = new MessageBoxDialogModel
        {
            Icon = MaterialIconKind.PersonQuestion,
            IconColor = Brushes.Gold,
            Title = "Delete profile?",
            Message =
                $"Are you sure you want to delete the selected profile `{model.SelectedProfile.Name}`? This action can not be undone. Do you want to continue?",
        };

        SukiHost.ShowDialog(msgBoxDialogModel);

        if (await msgBoxDialogModel.WaitForResultAsync() != UserConfirmation.Ok)
            return;

        var profileService = App.ServiceProvider.GetRequiredService<ProfileService>();
        profileService.Config.Profiles.Remove(model.SelectedProfile);
        
        model.SelectedProfile = profileService.Config.Profiles.First();
    }
}