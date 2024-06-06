using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRGame.Client;
using SRNetwork;

namespace SRCore;

public sealed class Kernel
{
    #region Events

    public delegate void KernelInitializedEventHandler(Kernel kernel);

    public event KernelInitializedEventHandler? KernelInitialized;

    public delegate void KernelShutdownEventHandler(Kernel kernel);

    public event KernelShutdownEventHandler? KernelShutdown;

    public delegate void GameInitializedEventHandler(Kernel kernel);

    public event GameInitializedEventHandler? GameInitialized;

    public delegate void StartLoadingEventHandler(Kernel kernel);

    public event StartLoadingEventHandler? StartLoading;

    public delegate void StopLoadingEventHandler(Kernel kernel);

    public event StopLoadingEventHandler? StopLoading;

    #endregion

    private readonly Proxy _proxy;
    private readonly ClientFileSystem _fileSystem;
    private readonly EntityManager _entityManager;
    private readonly ClientInfoManager _clientInfoManager;
    private readonly Profile _activeProfile;
    private readonly ClientlessManager _clientlessManager;
    private readonly IEnumerable<SRNetwork.MessageHandler> _messageHandlers;
    private readonly ProfileService _profileService;

    private readonly ILogger _logger;

    public Kernel(
        IServiceProvider serviceProvider,
        IEnumerable<SRNetwork.MessageHandler> messageHandlers,
        ILogger logger
    )
    {
        _logger = logger;
        _proxy = serviceProvider.GetRequiredService<Proxy>();
        _fileSystem = serviceProvider.GetRequiredService<ClientFileSystem>();
        _entityManager = serviceProvider.GetRequiredService<EntityManager>();
        _clientInfoManager = serviceProvider.GetRequiredService<ClientInfoManager>();
        _activeProfile = serviceProvider.GetRequiredService<Profile>();
        _clientlessManager = serviceProvider.GetRequiredService<ClientlessManager>();
        _messageHandlers = serviceProvider.GetServices<SRNetwork.MessageHandler>();
        _profileService = serviceProvider.GetRequiredService<ProfileService>();
        
        ServiceProvider = serviceProvider;

        Log.Logger = logger;
    }

    public static string ConfigDirectory { get; } = Path.Combine(Environment.CurrentDirectory, "user");

    private IServiceProvider ServiceProvider { get; set; }

    public bool IsInitialized { get; private set; }

    public bool IsGameInitialized { get; private set; }

    public bool IsNetworkInitialized => _proxy.Context != ProxyContext.None;

    public ClientType ClientType { get; private set; } = ClientType.Vietnam188;

    public async Task ShutdownAsync()
    {
        _logger.Debug("Shutting down SRKernel...");

        if (!IsInitialized)
        {
            throw new Exception("Kernel is not initialized. Use InitializeAsync before shutting down");
        }

        //FileSystem shutdown
        if (_fileSystem.IsInitialized)
        {
            await _fileSystem.Media!.CloseAsync();
            await _fileSystem.Data!.CloseAsync();
        }

        //Proxy shutdown
        if (_proxy.Context != ProxyContext.None)
        {
            await _proxy.ShutdownAsync();
        }

        _entityManager.Clear();

        IsInitialized = false;
        IsGameInitialized = false;

        OnKernelShutdown(this);
    }

    public async Task InitializeAsync()
    {
        OnStartLoading(this);

        _logger.Debug("SRKernel initializing...");

        if (IsInitialized)
        {
            throw new Exception("Kernel is already initialized. Use ShutdownAsync before reinitializing");
        }

        //Folder structure
        Directory.CreateDirectory(ConfigDirectory);

        await _profileService.LoadProfilesAsync().ConfigureAwait(false);
        _proxy.Initialize(_messageHandlers, _clientlessManager);

        IsInitialized = true;
        _logger.Debug("SRKernel initialized.");

        OnKernelInitialized(this);
        OnStopLoading(this);
    }

    public async Task InitializeGameAsync(string clientDirectory, ClientType clientType)
    {
        OnStartLoading(this);

        IsGameInitialized = false;
        ClientType = clientType;

        _logger.Information("Loading game...");

        //Init file system
        if (!_fileSystem.IsInitialized)
        {
            var mediaFile = Directory.GetFiles(clientDirectory, "media.pk2", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            var dataFile = Directory.GetFiles(clientDirectory, "data.pk2", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (mediaFile == null || dataFile == null)
            {
                throw new Exception("Game asset packs (media.pk2, data.pk2) not found!");
            }

            await _fileSystem.InitializeAsync(
                mediaFile,
                dataFile
            );
        }

        await _clientInfoManager.LoadAsync().ConfigureAwait(false);
        await _entityManager.LoadAsync(clientType).ConfigureAwait(false);

        _logger.Information("Game loaded.");

        IsGameInitialized = true;

        OnGameInitialized(this);
        OnStopLoading(this);
    }

    public async Task StartNetworkAsync()
    {
        //Testing around with NetEngine!
        var gatewayEndPoint =
            new IPEndPoint(IPAddress.Parse(_clientInfoManager.DivisionInfo.Divisions[0].GatewayServers[0]),
                _clientInfoManager.GatewayPort);

        if (_activeProfile is { Clientless: false })
        {
            await _proxy.StartClientProxy(16000);

            return;
        }

        await _proxy.ConnectToGateway(gatewayEndPoint);
    }

    private void OnKernelInitialized(Kernel kernel)
    {
        KernelInitialized?.Invoke(kernel);
    }

    private void OnKernelShutdown(Kernel kernel)
    {
        KernelShutdown?.Invoke(kernel);
    }

    private void OnGameInitialized(Kernel kernel)
    {
        GameInitialized?.Invoke(kernel);
    }

    private void OnStartLoading(Kernel kernel)
    {
        StartLoading?.Invoke(kernel);
    }

    private void OnStopLoading(Kernel kernel)
    {
        StopLoading?.Invoke(kernel);
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