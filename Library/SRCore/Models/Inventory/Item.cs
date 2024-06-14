using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory;

public abstract class Item(byte slot, RefObjItem item, ItemRentInfo? rentInfo = null) : ReactiveObject
{
    [Reactive] public RefObjItem RefObjItem { get; internal set; } = item;
    [Reactive] public byte Slot { get; internal set; } = slot;
    [Reactive] public uint RentType { get; internal set; }
    [Reactive] public ItemRentInfo? RentInfo { get; internal set; }
    [Reactive] public int RefObjId { get; internal set; } = item.Id;
    [Reactive] public ushort Quantity { get; internal set; } = 1;
}