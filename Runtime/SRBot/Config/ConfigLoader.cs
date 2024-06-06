using System.IO;
using System.Threading.Tasks;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRBot.Config;

public class ConfigLoader
{
    public async Task LoadConfigAsync(ConfigService configService, Profile profile)
    {
        _ = await configService.LoadConfigurationAsync(Path.Combine(profile.ConfigDirectory, LogConfig.FileName), new LogConfig());
    }
}