using System.IO;
using System.Threading.Tasks;
using SRCore.Config;
using SRCore.Config.Model;

namespace SRBot.Config;

public class AppConfigLoader(ConfigService configService)
{
    public async Task LoadConfigAsync(Profile profile)
    {
        _ = await configService.LoadConfigurationAsync(Path.Combine(profile.ConfigDirectory, LogConfig.FileName), new LogConfig());
    }
}