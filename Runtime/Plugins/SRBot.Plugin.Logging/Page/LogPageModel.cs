using System.Collections.ObjectModel;
using Material.Icons;
using Serilog;
using Serilog.Events;
using SRBot.Config;
using SRBot.Page;
using SRBot.Utils;
using SRCore.Config;

namespace SRBot.Plugin.Logging.Page;

public class LogPageModel : PageModel
{
    private readonly ConfigService _configService;
    private readonly ILogger _logger;

    public LogPageModel(MainThreadLogEventSink memoryLogEventSink, ConfigService configService, ILogger logger) : base("srbot_page_log", "Log", 99, MaterialIconKind.ConsoleLine)
    {
        _configService = configService;
        _logger = logger; 
        
        memoryLogEventSink.LogEventReceived += MemoryLogSinkOnLogEventReceived;
    }

    private void MemoryLogSinkOnLogEventReceived(LogEvent logEvent)
    {
        if (_configService.GetConfig<LogConfig>() is { RefreshLogView: false })
            return;
        
        Logs.Add(logEvent);
    }

    public ObservableCollection<LogEvent> Logs { get; } = new();

    public LogConfig Config => _configService.GetConfig<LogConfig>() ?? new LogConfig();

    public void ClearLogs()
    {
        Logs.Clear();
        
        _logger.Information("Logs cleared");
    }
}