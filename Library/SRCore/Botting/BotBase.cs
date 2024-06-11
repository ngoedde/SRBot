using ReactiveUI.Fody.Helpers;

namespace SRCore.Botting;

public abstract class BotBase(string name)
{
    public string Name { get; } = name;

    [Reactive] public BotState State { get; protected set; }
    
    public abstract void Start();
    public abstract void Stop();
    public abstract void Pause();
    public abstract void Resume();
    public abstract Task<bool> Tick();
}