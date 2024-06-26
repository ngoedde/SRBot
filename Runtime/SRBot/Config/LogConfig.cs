using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog.Events;

namespace SRBot.Config;

public class LogConfig : ReactiveObject
{
    public const string FileName = "log.json";

    [Reactive] public bool RefreshLogView { get; set; } = true;
    [Reactive] public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
}