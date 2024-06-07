using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRGame.Client;

namespace SRCore;

public sealed class Kernel
{
    public static string ConfigDirectory { get; } = Path.Combine(Environment.CurrentDirectory, "user");

    #region Events

    public delegate void KernelInitializedEventHandler(Kernel kernel);

    public event KernelInitializedEventHandler? KernelInitialized;

    public delegate void KernelShutdownEventHandler(Kernel kernel);

    public event KernelShutdownEventHandler? KernelShutdown;

    #endregion

    private readonly Proxy _proxy;
    private readonly Game _game;
    private readonly ClientlessManager _clientlessManager;
    private readonly IEnumerable<SRNetwork.MessageHandler> _messageHandlers;
    private readonly ProfileService _profileService;

    public Kernel(
        IServiceProvider serviceProvider,
        IEnumerable<SRNetwork.MessageHandler> messageHandlers)
    {
        _proxy = serviceProvider.GetRequiredService<Proxy>();
        _game = serviceProvider.GetRequiredService<Game>();
        _clientlessManager = serviceProvider.GetRequiredService<ClientlessManager>();
        _messageHandlers = serviceProvider.GetServices<SRNetwork.MessageHandler>();
        _profileService = serviceProvider.GetRequiredService<ProfileService>();
    }

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
        _proxy.Initialize(_messageHandlers, _clientlessManager);
        
        IsInitialized = true;

        OnKernelInitialized(this);

        Log.Debug("SRKernel initialized.");
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
        
        services.AddSingleton<Proxy>();
        services.AddSingleton<ClientlessManager>();
     
        return services;
    }
}