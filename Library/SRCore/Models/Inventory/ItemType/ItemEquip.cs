using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory.ItemType;

public class ItemEquip(byte slot, RefObjItem item, ItemRentInfo? rentInfo) : Item(slot, item, rentInfo)
{
    [Reactive] public byte OptLevel { get; internal set; }
    [Reactive] public ulong Variance { get; internal set; }
    [Reactive] public uint Durability { get; internal set; }
    [Reactive] public Dictionary<uint, uint> MagicParams { get; internal set; } = new();
    [Reactive] public byte BindingOptionTypeA { get; internal set; }
    [Reactive] public ObservableCollection<BindingOption> BindingOptionsA { get; internal set; } = new();
    [Reactive] public byte BindingOptionTypeB { get; internal set; }
    [Reactive] public ObservableCollection<BindingOption> BindingOptionsB { get; internal set; } = new();
}