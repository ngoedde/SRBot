using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Inventory;

public class TimedJob : ReactiveObject
{
    [Reactive] public byte Category { get; internal set; }
    [Reactive] public uint JobId { get; internal set; }
    [Reactive] public uint TimeToKeep { get; internal set; }
}