using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.ShardInfo;

public class Shard : ReactiveObject
{
    [Reactive] public ushort Id { get; init; }
    [Reactive] public string Name { get; init; }
    [Reactive] public ushort OnlineCount { get; init; }
    [Reactive] public ushort Capacity { get; init; }
    [Reactive] public bool Operating { get; init; }
    [Reactive] public byte FarmId { get; init; }
}