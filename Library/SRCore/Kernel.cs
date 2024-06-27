using System.Diagnostics;
using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Serilog.Events;
using SRCore.Botting;
using SRCore.Components;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRCore.Service;
using SRGame.Client;
using SRNetwork.Common;

namespace SRCore;

public sealed class Kernel(IServiceProvider serviceProvider, IScheduler scheduler)
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

    private readonly IEnumerable<SRNetwork.MessageHandler> _messageHandlers =
        serviceProvider.GetServices<SRNetwork.MessageHandler>();

    private readonly IEnumerable<SRNetwork.MessageHook> _messageHooks =
        serviceProvider.GetServices<SRNetwork.MessageHook>();

    private readonly ProfileService _profileService = serviceProvider.GetRequiredService<ProfileService>();
    private readonly ClientlessManager _clientlessManager = serviceProvider.GetRequiredService<ClientlessManager>();
    private readonly AccountService _accountService = serviceProvider.GetRequiredService<AccountService>();
    private readonly LoginService _loginService = serviceProvider.GetRequiredService<LoginService>();
    private readonly Bot _bot = serviceProvider.GetRequiredService<Bot>();
    private readonly MainLoopRegistry _mainLoopRegistry = serviceProvider.GetRequiredService<MainLoopRegistry>();

    private Stopwatch _stopwatch = new();

    public bool IsInitialized { get; private set; }
    public KernelMetrics Metrics { get; } = new();

    /// <summary>
    /// Initializes required services used by SRCore. This method should be called before using any other services.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public async Task RunAsync(CancellationToken token = default)
    {
        RxApp.MainThreadScheduler = scheduler;

        Log.Debug("SRKernel initializing...");

        if (IsInitialized)
            throw new Exception("Kernel is already initialized. Use ShutdownAsync before reinitializing");

        //Folder structure
        Directory.CreateDirectory(ConfigDirectory);

        await _profileService.LoadProfilesAsync().ConfigureAwait(false);
        await _accountService.Initialize("").ConfigureAwait(false);

        _proxy.Initialize(serviceProvider, scheduler);
        _loginService.Initialize();
        _clientlessManager.Initialize();
        _profileService.ActiveProfileChanged += ProfileServiceOnActiveProfileChanged;

        IsInitialized = true;

        OnKernelInitialized(this);
        
        Log.Debug("SRKernel initialized.");

        _stopwatch.Start();

        Run(token);
    }

    private void Run(CancellationToken token)
    {
        var lastGameTick = _stopwatch.ElapsedTicks;
        var nextGameTick = _stopwatch.ElapsedTicks;
        var frames = 0;
        var nextSecond = nextGameTick + Stopwatch.Frequency;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var frameStart = _stopwatch.ElapsedTicks;
                var ticksPerFrame = Stopwatch.Frequency / Metrics.TargetFPS;
            
                Metrics.TargetFrameTime = 1000.0 / Metrics.TargetFPS; // in milliseconds

                if (frameStart > nextGameTick)
                {
                    var ticksElapsed = frameStart - lastGameTick;
                
                    // Update game state
                    _mainLoopRegistry.Run(ticksElapsed);
                
                    //Time it took to process the frame
                    Metrics.FrameTime = (_stopwatch.ElapsedTicks - frameStart) * 1000.0 / Stopwatch.Frequency; // in milliseconds
                    Metrics.TotalFrames++;
                
                    nextGameTick += ticksPerFrame;
                    lastGameTick = _stopwatch.ElapsedTicks;
                    frames++;

                    if (_stopwatch.ElapsedTicks > nextSecond)
                    {
                        Metrics.FPS = frames;
                        frames = 0;
                        nextSecond += Stopwatch.Frequency;
                    }
                }
                else
                {
                    // Calculate idle time
                    var idleTime = (int)((nextGameTick - _stopwatch.ElapsedTicks) * 1000 / Stopwatch.Frequency);
                    Metrics.IdleTime = idleTime;

                    Thread.Sleep(idleTime);
                }
                //
                // Debug.WriteLine($"FPS: {Metrics.FPS}, " +
                //                 $"FrameTime: {Math.Round(Metrics.FrameTime, 2)}ms ({Math.Round(Metrics.FrameTime / Metrics.TargetFrameTime * 100,2)}%), " +
                //                 $"IdleTime: {Math.Round(Metrics.IdleTime, 2)}ms ({Math.Round(Metrics.IdleTime / Metrics.TargetFrameTime * 100,2)}%)");
            }
        }
        catch (Exception e)
        {
            _ = Panic(e.Message, LogEventLevel.Fatal);
        }

    }

    private void ProfileServiceOnActiveProfileChanged(Profile profile)
    {
        OnInterrupt(this, "Active profile has changed.", LogEventLevel.Information);
    }

    public async Task Panic(string message, LogEventLevel level = LogEventLevel.Fatal, bool shutdown = true)
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
        _game.Close();
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
        services.AddSingleton<MainLoopRegistry>();

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