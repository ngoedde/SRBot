using Avalonia.Threading;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace SRBot.Utils;

public class MainThreadLogEventSink : ILogEventSink
{
    public delegate void LogEventReceivedEventHandler(LogEvent logEvent);
    public event LogEventReceivedEventHandler? LogEventReceived;
    
    public void Emit(LogEvent logEvent)
    {
        OnLogEventReceived(logEvent);
    }

    protected virtual void OnLogEventReceived(LogEvent logevent)
    {
        //Pass action to the UI thread to keep threads sync.
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            LogEventReceived?.Invoke(logevent);
        });
    }
}

public static class LogSinkExtensions
{
    public static LoggerConfiguration InMemorySink(this LoggerSinkConfiguration loggerSinkConfiguration, MainThreadLogEventSink eventSink)
    {
        return loggerSinkConfiguration.Sink(eventSink);
    }
}