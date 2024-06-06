using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.ShardInfo;

public class Farm : ReactiveObject
{
    [Reactive] public byte Id { get; init; }
    [Reactive] public string Name { get; init; }
}