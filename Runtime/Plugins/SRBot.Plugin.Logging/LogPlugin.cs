using Material.Icons;
using Microsoft.Extensions.DependencyInjection;

namespace SRBot.Plugin.Logging;

public class LogPlugin : AppPlugin
{
    public LogPlugin()
    {
        TechnicalName = typeof(LogPlugin).FullName ?? DisplayName;
        DisplayName = "Log";
        Icon = MaterialIconKind.ErrorOutline;
    }
}