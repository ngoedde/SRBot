using System.Text.Json;
using ReactiveUI.Fody.Helpers;
using SRCore.Config.Model;

namespace SRCore.Config;

public sealed class ProfileService
{
    private readonly ConfigService _configService;

    public ProfileService(ConfigService configService)
    {
        _configService = configService;
    }

    #region Events

    public delegate void ActiveProfileChangedEventHandler(Profile profile);

    public event ActiveProfileChangedEventHandler? ActiveProfileChanged;

    public delegate void ProfileConfigLoadedEventHandler(ProfileConfig config);

    public event ProfileConfigLoadedEventHandler? ProfileConfigLoaded;

    #endregion

    #region Properties

    public string ProfileConfigPath => Path.Combine(Kernel.ConfigDirectory, "profiles.json");

    [Reactive] public ProfileConfig Config => _configService.GetConfig<ProfileConfig>() ?? GetDefaultProfileConfig();

    [Reactive] public Profile ActiveProfile { get; private set; }

    #endregion

    public async Task SaveProfilesAsync(string? path = null)
    {
        path ??= ProfileConfigPath;
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var jsonString = JsonSerializer.Serialize(Config, options);

        await File.WriteAllTextAsync(path, jsonString);
    }

    public async Task LoadProfilesAsync(string? path = null)
    {
        await _configService.LoadConfigurationAsync(ProfileConfigPath, GetDefaultProfileConfig());

        path ??= ProfileConfigPath;
        if (!File.Exists(ProfileConfigPath))
        {
            await SaveProfilesAsync(path);
        }

        //Add default profile if there are no profiles
        if (Config.Profiles.Count == 0)
            Config.Profiles.Add(GetDefaultProfile());

        await SetActiveProfileAsync(Config.ActiveProfile);

        OnProfileConfigLoaded(Config);
    }


    public async Task SetActiveProfileAsync(Profile profile)
    {
        Config.ActiveProfile = profile.Name;
        ActiveProfile = profile;
        
        await LoadActiveProfileConfigs(profile.ConfigDirectory);

        OnProfileChanged(profile);
    }

    public async Task SetActiveProfileAsync(string name)
    {
        var profile = GetProfile(name);
        if (profile == null)
            return;

        await SetActiveProfileAsync(profile);
    }

    public Profile? GetProfile(string name) => Config.Profiles.FirstOrDefault(p => p.Name == name);

    private async Task LoadActiveProfileConfigs(string configDirectory)
    {
        _ = await _configService.LoadConfigurationAsync(Path.Combine(configDirectory, GameConfig.FileName), new GameConfig()).ConfigureAwait(false);
    }

    private void OnProfileChanged(Model.Profile profile)
        => ActiveProfileChanged?.Invoke(profile);


    private void OnProfileConfigLoaded(ProfileConfig config)
        => ProfileConfigLoaded?.Invoke(config);
    
    private static ProfileConfig GetDefaultProfileConfig()
    {
        return new ProfileConfig
        {
            Profiles = [GetDefaultProfile()],
            ActiveProfile = ProfileConfig.DefaultProfileName
        };
    }

    private static Profile GetDefaultProfile()
    {
        return new Profile(Path.Combine(Kernel.ConfigDirectory, ProfileConfig.DefaultProfileName), ProfileConfig.DefaultProfileName)
        {
            Description = "The default profile."
        };
    }
}