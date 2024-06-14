using Serilog;

namespace SRCore.Botting.Bases;

public class TrainingBot() : BotBase("SRCore.Bot.Training")
{
    public override void Start()
    {
        State = BotState.Started;

        Log.Debug("TrainingBot Start");
    }

    public override void Stop()
    {
        State = BotState.Idle;

        Log.Debug("TrainingBot Stop");
    }

    public override void Pause()
    {
        State = BotState.Paused;

        Log.Debug("TrainingBot Pause");
    }

    public override void Resume()
    {
        State = BotState.Started;

        Log.Debug("TrainingBot Resume");
    }

    public override Task<bool> Tick()
    {
        return Task.FromResult(true);
    }
}