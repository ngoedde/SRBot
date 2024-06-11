using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory.ItemType;

public class ItemExpendable(byte slot, RefObjItem item, ItemRentInfo rentInfo) : Item(slot, item, rentInfo)
{
    [Reactive] public ushort Quantity { get; internal set; }
    [Reactive] public byte AttributeAssimilationProbability { get; internal set; }

    [Reactive] public Dictionary<uint, uint> MagicParams { get; internal set; } = new();
}