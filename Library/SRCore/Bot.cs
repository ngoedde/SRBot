using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using SRCore.Botting;
using SRCore.Service;

namespace SRCore;

public class Bot : ReactiveObject
{
    public delegate void BotStoppedEventHandler(BotBase bot);

    public delegate void BotStartedEventHandler(BotBase bot);

    public delegate void BotPausedEventHandler(BotBase bot);

    public event BotStartedEventHandler? BotStarted;

    public event BotStoppedEventHandler? BotStopped;

    public event BotPausedEventHandler? BotPaused;

    [Reactive] public int TicksPerSecond { get; private set; } = 0;
    [Reactive] public int DesiredTicksPerSecond { get; set; } = 30;
    [Reactive] public BotBase? CurrentBot { get; set; }

    public IEnumerable<BotBase> Bots { get; }
    public BotState State => CurrentBot?.State ?? BotState.Idle;


    public Bot(IEnumerable<BotBase> botBases, MainLoopRegistry loopRegistry)
    {
        Bots = botBases;
        CurrentBot = Bots.FirstOrDefault();
        DesiredTicksPerSecond = 30;

        loopRegistry.Register(Tick);
    }

    public async Task Tick(long delta)
    {
        if (State != BotState.Started)
            return;

        //Error in bot tick?
        if (!await CurrentBot!.Tick())
            StopBot();
        
    }

    public void StartBot()
    {
        if (CurrentBot == null)
            return;

        CurrentBot.Start();

        OnBotStarted(CurrentBot);
    }

    public void StopBot()
    {
        if (CurrentBot == null)
            return;

        CurrentBot.Stop();

        OnBotStopped(CurrentBot);
    }

    public void PauseBot()
    {
        if (CurrentBot == null)
            return;

        CurrentBot.Pause();

        OnBotPaused(CurrentBot);
    }

    protected virtual void OnBotStopped(BotBase bot)
    {
        BotStopped?.Invoke(bot);

        Log.Information("Bot stopped");
    }

    protected virtual void OnBotStarted(BotBase bot)
    {
        BotStarted?.Invoke(bot);

        Log.Information("Bot started");
    }

    protected virtual void OnBotPaused(BotBase bot)
    {
        BotPaused?.Invoke(bot);

        Log.Information("Bot paused");
    }
}