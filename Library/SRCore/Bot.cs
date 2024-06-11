using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using SRCore.Botting;
using Timer = System.Timers.Timer;

namespace SRCore;

public class Bot(IEnumerable<BotBase> botBases): ReactiveObject
{
    public delegate void BotStoppedEventHandler(BotBase bot);
    public delegate void BotStartedEventHandler(BotBase bot);
    public delegate void BotPausedEventHandler(BotBase bot);
    
    public event BotStartedEventHandler? BotStarted;
    
    public event BotStoppedEventHandler? BotStopped;
    
    public event BotPausedEventHandler? BotPaused;
    
    [Reactive] public int TicksPerSecond { get; private set; } = 0;
    [Reactive] public int DesiredTicksPerSecond { get; set; } = 30;
    [Reactive] public BotBase? CurrentBot { get; set; } = botBases.FirstOrDefault();
    
    public IEnumerable<BotBase> Bots { get; } = botBases;
    public BotState State => CurrentBot?.State ?? BotState.Idle;
    
    //used for ticks/sec calculation
    private Task _tickTask = Task.CompletedTask;
    private int _tickCount = 0;
    private Timer _tickTimer = new(1000);
    
    public async void Tick()
    {
        var delayMilliseconds = 1000 / DesiredTicksPerSecond;
        
        while (State == BotState.Started)
        {
            //Error in bot tick?
            if(!await CurrentBot!.Tick())
                StopBot();
            
            await Task.Delay(delayMilliseconds);
            _tickCount++;
        }
    }

    public void StartBot()
    {
        if (CurrentBot == null)
            return;
        
        CurrentBot.Start();

        _tickTask = new Task(Tick);
        _tickTimer.Elapsed += (sender, e) => 
        {
            TicksPerSecond = _tickCount;
            _tickCount = 0; // Reset the counter
        };
        _tickTimer.Start();
        _tickTask.Start(TaskScheduler.Current);
        
        OnBotStarted(CurrentBot);
    }
    
    public void StopBot()
    {
        if (CurrentBot == null)
            return;
        
        CurrentBot.Stop();
        
        _tickTask.Dispose();
        _tickTimer.Stop();
        
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