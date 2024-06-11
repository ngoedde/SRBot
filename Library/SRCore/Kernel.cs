using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SRCore.Botting;
using SRCore.Components;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRCore.Service;
using SRGame.Client;

namespace SRCore;

public sealed class Kernel(
    IServiceProvider serviceProvider)
{
    public static string ConfigDirectory { get; } = Path.Combine(Environment.CurrentDirectory, "user");

    #region Events

    public delegate void KernelInitializedEventHandler(Kernel kernel);

    public event KernelInitializedEventHandler? KernelInitialized;

    public delegate void KernelShutdownEventHandler(Kernel kernel);

    public event KernelShutdownEventHandler? KernelShutdown;

    public delegate void InterruptEventHandler(Kernel kernel, string message, LogEventLevel level);
    public event InterruptEventHandler? Interrupt;
    
    #endregion

    private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();
    private readonly Game _game = serviceProvider.GetRequiredService<Game>();
    private readonly IEnumerable<SRNetwork.MessageHandler> _messageHandlers = serviceProvider.GetServices<SRNetwork.MessageHandler>();
    private readonly IEnumerable<SRNetwork.MessageHook> _messageHooks = serviceProvider.GetServices<SRNetwork.MessageHook>();
    private readonly ProfileService _profileService = serviceProvider.GetRequiredService<ProfileService>();
    private readonly ClientlessManager _clientlessManager = serviceProvider.GetRequiredService<ClientlessManager>();
    private readonly AccountService _accountService = serviceProvider.GetRequiredService<AccountService>();
    private readonly LoginService _loginService = serviceProvider.GetRequiredService<LoginService>();
    private readonly Bot _bot = serviceProvider.GetRequiredService<Bot>();

    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Initializes required services used by SRCore. This method should be called before using any other services.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public async Task InitializeAsync()
    {
        Log.Debug("SRKernel initializing...");

        if (IsInitialized)
            throw new Exception("Kernel is already initialized. Use ShutdownAsync before reinitializing");
        
        //Folder structure
        Directory.CreateDirectory(ConfigDirectory);
        
        await _profileService.LoadProfilesAsync().ConfigureAwait(false);
        await _accountService.Initialize("").ConfigureAwait(false);
        
        _proxy.Initialize(_messageHandlers, _messageHooks);
        _loginService.Initialize();
        _clientlessManager.Initialize();
        _profileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;
        
        IsInitialized = true;

        OnKernelInitialized(this);

        Log.Debug("SRKernel initialized.");
    }

    private void ProfileServiceOnActiveProfileChanged(Profile profile)
    {
        OnInterrupt(this, "Active profile has changed.", LogEventLevel.Information);
    }

    public async Task TriggerInterrupt(string message, LogEventLevel level = LogEventLevel.Fatal, bool shutdown = true)
    {
        OnInterrupt(this, message, level);

        if (shutdown)
        {
            await _proxy.ShutdownAsync();
            _bot.StopBot();
        }
        
        Log.Write(level, message);
    }
    
    /// <summary>
    /// Shuts down the SRKernel and all services it depends on.
    /// </summary>
    public async Task ShutdownAsync()
    {
        Log.Debug("Shutting down SRKernel...");

        //Game shutdown
        await _game.CloseAsync();
        await _proxy.ShutdownAsync();
        
        IsInitialized = false;

        OnKernelShutdown(this);
    }
    
    private void OnKernelInitialized(Kernel kernel)
    {
        KernelInitialized?.Invoke(kernel);
    }

    private void OnKernelShutdown(Kernel kernel)
    {
        KernelShutdown?.Invoke(kernel);
    }

    private void OnInterrupt(Kernel kernel, string message, LogEventLevel level)
    {
        Interrupt?.Invoke(kernel, message, level);
        
        Log.Debug($"[{level}] Kernel interrupt: {message}");
    }
}

public static class KernelExtensions
{
    public static IServiceCollection AddSRKernel(this IServiceCollection services)
    {
        services.AddSingleton<Kernel>();
        services.AddSingleton<ClientFileSystem>();
        services.AddSingleton<ConfigService>();
        services.AddSingleton<ProfileService>();
        services.AddSingleton<Game>();
        //Add the "Active Profile" to the DIC for easy access
        services.AddSingleton<Profile>(provider =>
        {
            var profileService = provider.GetRequiredService<ProfileService>();
            
            return profileService.ActiveProfile;
        });
        
        services.AddSingleton<EntityManager>();
        services.AddSingleton<ClientInfoManager>();
        
        //Game models
        var gameModels = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => !p.IsAbstract && typeof(GameModel).IsAssignableFrom(p));
        foreach (var type in gameModels)
            services.AddSingleton(type);
        
        //Networking
        var packetHandler = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => !p.IsAbstract && typeof(SRNetwork.MessageHandler).IsAssignableFrom(p));
        foreach (var type in packetHandler)
            services.AddSingleton(typeof(SRNetwork.MessageHandler), type);

        var packetHooks = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => !p.IsAbstract && typeof(SRNetwork.MessageHook).IsAssignableFrom(p));
        foreach (var type in packetHooks)
            services.AddSingleton(typeof(SRNetwork.MessageHook), type);


        //Bot bases
        var botBases = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => !p.IsAbstract && typeof(BotBase).IsAssignableFrom(p));
        foreach (var type in botBases)
            services.AddSingleton(typeof(BotBase), type);
        
        services.AddSingleton<Bot>();
        services.AddSingleton<Proxy>();
        services.AddSingleton<ClientlessManager>();
        services.AddSingleton<AccountService>();
        services.AddSingleton<LoginService>();
     
        return services;
    }
}