using ReactiveUI.Fody.Helpers;

namespace SRCore.Bot;

public abstract class BotBase(string name)
{
    public string Name { get; } = name;

    [Reactive] public BotState State { get; private set; }
    
    public abstract void Start();
    public abstract void Stop();
    public abstract void Pause();
}