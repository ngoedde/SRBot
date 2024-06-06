using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRBot.Dialog;

public class ProfileDialogModel : ViewModel
{
    private readonly ProfileService _profileService;

    public ProfileDialogModel(ProfileService profileService)
    {
        _profileService = profileService;

        SelectedProfile = _profileService.ActiveProfile;
    }

    [Reactive]
    public List<Profile> Profiles => _profileService.Config.Profiles;

    [Reactive]
    public Profile? SelectedProfile { get; set; }

    public async Task SetActiveProfile(Profile profile)
    {
        await _profileService.SetActiveProfileAsync(profile);
    }
    
    public async Task SaveProfiles()
    {
        await _profileService.SaveProfilesAsync();
    }

    public void AddProfile(string name= "New profile")
    {
        var profile = new Profile(Kernel.ConfigDirectory, name);
        _profileService.Config.Profiles.Add(profile);

        this.RaisePropertyChanged(nameof(Profiles));
        this.SelectedProfile = profile;
    }

}