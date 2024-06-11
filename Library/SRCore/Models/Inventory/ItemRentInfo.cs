using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Inventory;

public class ItemRentInfo : ReactiveObject
{
    [Reactive] public ushort CanDelete { get; internal set; }
    [Reactive] public ushort CanRecharge { get; internal set; }
    [Reactive] public uint PeriodBeginTime { get; internal set; }
    [Reactive] public uint PeriodEndTime { get; internal set; }
    [Reactive] public uint MeterRateTime { get; internal set; }
    [Reactive] public uint PackingTime { get; internal set; }
}