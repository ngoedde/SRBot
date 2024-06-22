using Avalonia;
using System;
using Avalonia.Dialogs;
using System.Diagnostics;

namespace SRBot;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        KillProcessesByName("sro_client");
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static void KillProcessesByName(string processName)
    {
        // Abrufen aller Prozesse im System
        Process[] processes = Process.GetProcesses();

        // Durchlaufen aller Prozesse
        foreach (var process in processes)
        {
            // Vergleich des Prozessnamens mit dem gesuchten Namen, unabhängig von Groß- und Kleinschreibung
            if (string.Equals(process.ProcessName, processName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Versuch, den Prozess zu beenden
                    process.Kill();
                    // Warten, bis der Prozess beendet ist
                    process.WaitForExit();
                    Console.WriteLine($"Prozess {processName} wurde erfolgreich beendet.");
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung, falls der Prozess nicht beendet werden kann
                    Console.WriteLine($"Fehler beim Beenden des Prozesses {processName}: {ex.Message}");
                }
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseSkia()
            .LogToTrace();
}