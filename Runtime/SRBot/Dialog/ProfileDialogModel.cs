using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRBot.Dialog;

public class ProfileDialogModel : ViewModel
{
    public ProfileService ProfileService { get; }
    
    public Kernel Kernel { get; }

    public ProfileDialogModel(ProfileService profileService, Kernel kernel)
    {
        ProfileService = profileService;
        SelectedProfile = ProfileService.ActiveProfile;
        Kernel = kernel;
    }
    
    public IEnumerable<Profile> Profiles => ProfileService.Config.Profiles;

    [Reactive]
    public Profile SelectedProfile { get; set; }

    public async Task SetActiveProfile(Profile profile)
    {
        await ProfileService.SetActiveProfileAsync(profile);
    }
    
    public async Task SaveProfiles()
    {
        await ProfileService.SaveProfilesAsync();
    }

    public void AddProfile(string name= "New profile")
    {
        var profile = new Profile(Path.Combine(Kernel.ConfigDirectory, name), name);
        ProfileService.Config.Profiles.Add(profile);

        SelectedProfile = profile;
        
        // this.RaisePropertyChanged(nameof(Profiles));
    }

    public void DeleteProfile(Profile profile)
    {
        ProfileService.Config.Profiles.Remove(profile);
        
        // this.RaisePropertyChanged(nameof(Profiles));
    }
}
