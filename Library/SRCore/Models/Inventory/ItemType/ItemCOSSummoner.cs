using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory.ItemType;

public class ItemCosSummoner(byte slot, RefObjItem item, ItemRentInfo? rentInfo) : Item(slot, item, rentInfo)
{
    [Reactive] public SummonerState State { get; internal set; }
    [Reactive] public uint ContainedRefObjId { get; internal set; }
    [Reactive] public string Name { get; internal set; } = string.Empty;
    [Reactive] public uint SecondsToRentEndTime { get; internal set; }
    [Reactive] public ObservableCollection<TimedJob> TimedJobs { get; internal set; } = new();
}