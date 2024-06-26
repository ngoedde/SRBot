using System.Collections.ObjectModel;
using Material.Icons;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Serilog.Events;
using SRBot.Config;
using SRBot.Utils;
using SRCore.Config;

namespace SRBot.Page.Logging;

public class LogPageModel : PageModel
{
    private readonly ConfigService _configService;

    [Reactive] public bool ScrollToEnd { get; set; } = true;

    public LogPageModel(MainThreadLogEventSink memoryLogEventSink, ConfigService configService) : base("srbot_page_log",
        "Log", 99, MaterialIconKind.ConsoleLine)
    {
        _configService = configService;

        memoryLogEventSink.LogEventReceived += MemoryLogSinkOnLogEventReceived;
    }

    private void MemoryLogSinkOnLogEventReceived(LogEvent logEvent)
    {
        if (_configService.GetConfig<LogConfig>() is { RefreshLogView: false })
            return;
        
        if (Logs.Count > 1000)
            Logs.RemoveAt(0);
        
        Logs.Add(logEvent);
    }

    public ObservableCollection<LogEvent> Logs { get; } = new();
    
    public LogConfig Config => _configService.GetConfig<LogConfig>() ?? new LogConfig();

    public void ClearLogs()
    {
        Logs.Clear();

        Log.Information("Logs cleared");
    }
}