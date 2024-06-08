using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Bot;

public class BotManager(IEnumerable<BotBase> botBases)
{
    [Reactive] public BotBase CurrentBot { get; set; } = botBases.First();
    
    public IEnumerable<BotBase> Bots { get; } = botBases;
    
    public void StartBot()
    {
        CurrentBot?.Start();
    }
    
    public void StopBot()
    {
        CurrentBot?.Stop();
    }
    
    public void PauseBot()
    {
        CurrentBot?.Pause();
    }
}