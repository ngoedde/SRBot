using System.Text.Json;
using SRCore.Config.Model;

namespace SRCore.Config;

public sealed class ProfileService(ConfigService configService)
{
    #region Events

    public delegate void ActiveProfileChangedEventHandler(Profile profile);

    public event ActiveProfileChangedEventHandler? ActiveProfileChanged;

    public delegate void ProfileConfigLoadedEventHandler(ProfileConfig config);

    public event ProfileConfigLoadedEventHandler? ProfileConfigLoaded;

    #endregion

    #region Properties

    public string ProfileConfigPath => Path.Combine(Kernel.ConfigDirectory, "profiles.json");

    public ProfileConfig Config => configService.GetConfig<ProfileConfig>() ?? new ProfileConfig();

    public Profile ActiveProfile { get; private set; } = new(Kernel.ConfigDirectory, "default");

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
        await configService.LoadConfigurationAsync(ProfileConfigPath, GetDefaultProfileConfig());

        path ??= ProfileConfigPath;
        if (!File.Exists(ProfileConfigPath))
        {
            await SaveProfilesAsync(path);
        }

        OnProfileConfigLoaded(Config);
    }


    public async Task SetActiveProfileAsync(Model.Profile profile)
    {
        if (ActiveProfile != null)
        {
            await configService.SaveAllAsync();
        }

        await LoadActiveProfileConfigs(profile.ConfigDirectory);

        Config.ActiveProfile = profile.Name;
        ActiveProfile = profile;

        OnProfileChanged(profile);
    }

    public async Task SetActiveProfileAsync(string name)
    {
        var profile = GetProfile(name);
        if (profile == null)
        {
            throw new Exception($"Profile {name} not found");
        }

        await SetActiveProfileAsync(profile);
    }

    public Model.Profile? GetProfile(string name) => Config.Profiles.FirstOrDefault(p => p.Name == name);

    private async Task LoadActiveProfileConfigs(string configDirectory)
    {
        _ = await configService.LoadConfigurationAsync(Path.Combine(configDirectory, GameConfig.FileName), new GameConfig()).ConfigureAwait(false);
    }

    private void OnProfileChanged(Model.Profile profile)
        => ActiveProfileChanged?.Invoke(profile);


    private void OnProfileConfigLoaded(ProfileConfig config)
        => ProfileConfigLoaded?.Invoke(config);
    
    private static ProfileConfig GetDefaultProfileConfig()
    {
        var defaultProfileConfig = new ProfileConfig();
        defaultProfileConfig.Profiles.Add(new Model.Profile(Path.Combine(Kernel.ConfigDirectory, "default"), "default"));

        return defaultProfileConfig;
    }
}