using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Material.Icons;
using ReactiveUI.Fody.Helpers;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
using SukiUI.Controls;
using SukiUI.Enums;

namespace SRBot.Dialog;

public class ProfileDialogModel : ViewModel
{
    private readonly ProfileService _profileService;
    private readonly Game _game;

    #region Properties

    public IEnumerable<Profile> Profiles => _profileService.Config.Profiles;

    [Reactive] public Profile SelectedProfile { get; set; }

    #endregion

    public ProfileDialogModel(ProfileService profileService, Game game)
    {
        _profileService = profileService;
        _game = game;

        SelectedProfile = _profileService.ActiveProfile;
    }

    #region Methods

    public async Task SetActiveProfile(Profile profile)
    {
        await _profileService.SetActiveProfileAsync(profile);
    }

    public async Task SaveProfiles()
    {
        await _profileService.SaveProfilesAsync();
    }

    public void AddProfile(string name = "New profile")
    {
        var profile = new Profile(Path.Combine(Kernel.ConfigDirectory, name), name);
        _profileService.Config.Profiles.Add(profile);

        SelectedProfile = profile;
    }

    public async Task DeleteProfile(Profile profile)
    {
        if (_profileService.ActiveProfile == profile)
        {
            await SukiHost.ShowToast("Error", "You cannot delete the active profile.", SukiToastType.Error);

            return;
        }

        var result = await App.MessageBoxManager.ShowMessageBoxAsync("Delete profile",
            $"Are you sure you want to delete the profile '{profile.Name}'?",
            MessageBoxButtons.Yes | MessageBoxButtons.No, MaterialIconKind.PersonQuestion, Brushes.Gold);

        if (result != MessageBoxDialogResult.Ok)
            return;

        _profileService.Config.Profiles.Remove(profile);

        await _profileService.SaveProfilesAsync();
    }

    public async Task LoadProfile(Profile profile)
    {
        if (_game.IsLoaded && _profileService.ActiveProfile != profile)
        {
            var result = await App.MessageBoxManager.ShowMessageBoxAsync("The game is already initialized.",
                "The game is already initialized. If you choose a different profile, the bot will reload all data. Do you want to continue?",
                MessageBoxButtons.Yes | MessageBoxButtons.No, MaterialIconKind.MessageWarning, Brushes.Gold);

            if (result != MessageBoxDialogResult.Ok)
                return;
        }

        SukiHost.CloseDialog();
        
        if (_profileService.ActiveProfile != profile){
            await SetActiveProfile(profile);
        }
        
        await SaveProfiles();
    }

    public async Task BrowseClientFolder()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = App.GetTopLevel();
        if (topLevel == null)
        {
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

        SelectedProfile.ClientDirectory = selectedFolder;
    }

    public void CloseDialog()
    {
        SukiHost.CloseDialog();
    }

    #endregion
}