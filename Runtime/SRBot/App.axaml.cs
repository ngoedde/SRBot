using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SRBot.Config;
using SRBot.Page;
using SRBot.Utils;
using SRCore;
using SRCore.Config;
using SRCore.Config.Model;
using ViewLocator = SRBot.Utils.ViewLocator;

namespace SRBot;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    private Kernel? _kernel;
    private static ConfigLoader ConfigLoader => ServiceProvider.GetRequiredService<ConfigLoader>();
    private static ConfigService ConfigService => ServiceProvider.GetRequiredService<ConfigService>();
    private static ProfileService ProfileService => ServiceProvider.GetRequiredService<ProfileService>();

    public static MainWindow? MainWindow;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        ServiceProvider = ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ProfileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;

        _kernel = ServiceProvider.GetRequiredService<Kernel>();
        _kernel.InitializeAsync().ConfigureAwait(false);
        
        _kernel.KernelInitialized += KernelOnKernelInitialized;
        _kernel.KernelShutdown += KernelOnKernelShutdown;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewLocator = ServiceProvider.GetRequiredService<IDataTemplate>();
            var mainVm = ServiceProvider.GetRequiredService<MainWindowModel>();
            
            MainWindow = viewLocator.Build(mainVm) as MainWindow;
            desktop.MainWindow = MainWindow;
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
        var pluginLoader = ServiceProvider.GetRequiredService<AppPluginLoader>();

        foreach (var plugin in pluginLoader.GetPlugins())
            plugin.Initialize(kernel);
    }

    private async void ProfileServiceOnActiveProfileChanged(Profile profile)
    {
        await ConfigLoader.LoadConfigAsync(ConfigService, profile);
    }

    private static ServiceProvider ConfigureServices()
    {        
        var services = new ServiceCollection();
        
        //Logger
        var memorySink = new MainThreadLogEventSink();
        services.AddSingleton(memorySink);
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.InMemorySink(memorySink)
            // .WriteTo.File("log.txt")
            .CreateLogger();
        
        //Services
        var pluginLoader = new AppPluginLoader(logger);
        services.AddSingleton<ILogger>(logger);
        services.AddSRKernel();
        services.AddSingleton(pluginLoader);
        services.AddSingleton<ConfigLoader>();
        services.AddSingleton<PageNavigationService>();
        
        // View models
        Current?.DataTemplates.Add(new ViewLocator(pluginLoader));
        var viewlocator = Current?.DataTemplates.First(x => x is ViewLocator);
        if (viewlocator is not null)
            services.AddSingleton(viewlocator);
        services.AddSingleton<MainWindowModel>();
        
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
}