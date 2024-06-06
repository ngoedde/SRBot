using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRBot.Dialog;

public class ProfileDialogModel : ViewModel
{
    private readonly ProfileService _profileService;

    public ProfileDialogModel(ProfileService profileService)
    {
        _profileService = profileService;
        SelectedProfile = _profileService.Config.ActiveProfile;
    }

    public List<Profile> Profiles => _profileService.Config.Profiles;
    
    public IEnumerable<string> ProfileNames => Profiles.Select(x => x.Name);
    
    public string SelectedProfile { get; set; }
    
    public async Task SetActiveProfile(string name)
    {
        var profile = _profileService.GetProfile(name);
        if (profile == null)
        {
            return;
        }
        
        await _profileService.SetActiveProfileAsync(profile);
    }
    
    public async Task SaveProfiles()
    {
        await _profileService.SaveProfilesAsync();
    }
}