using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using SRBot.Config;
using SRBot.Dialog;
using SRBot.Page;
using SRBot.Utils;
using SRCore;
using ILogger = Serilog.ILogger;
using ViewLocator = SRBot.Utils.ViewLocator;

namespace SRBot;

public partial class App : Application
{
    public static LoggingLevelSwitch LoggingLevelSwitch { get; } = new();
    public static MessageBoxManager MessageBoxManager => ServiceProvider.GetRequiredService<MessageBoxManager>();

    private static Kernel Kernel => ServiceProvider.GetRequiredService<Kernel>();
    private static IServiceProvider ServiceProvider { get; set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        ServiceProvider = ConfigureServices();
        Kernel.KernelInitialized += KernelOnKernelInitialized;
        Kernel.KernelShutdown += KernelOnKernelShutdown;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Kernel.InitializeAsync().ConfigureAwait(false);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewLocator = ServiceProvider.GetRequiredService<IDataTemplate>();

            var mainVm = ServiceProvider.GetRequiredService<MainWindowModel>();
            desktop.MainWindow = viewLocator.Build(mainVm) as MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void KernelOnKernelShutdown(Kernel kernel)
    {
        var pluginLoader = ServiceProvider.GetRequiredService<AppPluginLoader>();

        foreach (var plugin in pluginLoader.GetPlugins())
            plugin.Shutdown(kernel);
    }

    private void KernelOnKernelInitialized(Kernel kernel)
    {
        //Load log level from config -> Set logging level application wide after the kernel is initialized and the active profile loaded.
        var pluginLoader = ServiceProvider.GetRequiredService<AppPluginLoader>();

        foreach (var plugin in pluginLoader.GetPlugins())
            plugin.Initialize(kernel);
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        ;
        //Logger
        var memorySink = new MainThreadLogEventSink();
        services.AddSingleton(memorySink);

        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch)
            .WriteTo.Console()
            .WriteTo.InMemorySink(memorySink)
            .WriteTo.File(
                Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            ).CreateLogger();

        //Use static logger -> DI logger is still possible by passing an ILogger object
        Log.Logger = logger;

        //Services
        var pluginLoader = new AppPluginLoader(logger);
        services.AddSingleton<ILogger>(logger);
        services.AddSRKernel();
        services.AddSingleton(pluginLoader);
        services.AddSingleton<AppConfigLoader>();
        services.AddSingleton<MessageBoxManager>();
        services.AddSingleton<ViewLocator>();
        // services.AddSingleton<PageNavigationService>();

        // View models
        Current?.DataTemplates.Add(new ViewLocator(pluginLoader));
        var viewlocator = Current?.DataTemplates.First(x => x is ViewLocator);
        if (viewlocator is not null)
            services.AddSingleton(viewlocator);
        services.AddSingleton<MainWindowModel>();
        services.AddSingleton<ServerListDialogModel>();
        services.AddSingleton<ClientInfoDialogModel>();

        //Plugins
        var plugins = pluginLoader.LoadPlugins();
        foreach (var plugin in plugins)
        {
            plugin.BuildServices(services);
            services.AddSingleton(typeof(AppPlugin), plugin);
        }

        //Pages
        var pageExtensionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => !p.IsAbstract && typeof(PageModel).IsAssignableFrom(p));
        foreach (var type in pageExtensionTypes)
            services.AddSingleton(typeof(PageModel), type);

        return services.BuildServiceProvider();
    }

    public static WindowBase? GetTopLevel()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            return desktopLifetime.MainWindow;
        }

        return null;
    }
}