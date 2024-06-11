using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory.ItemType;

public class ItemMonsterCapsule(byte slot, RefObjItem item, ItemRentInfo rentInfo) : ItemContainer(slot, item, rentInfo)
{
    [Reactive] public uint ContainedRefObjId { get; internal set; }
}