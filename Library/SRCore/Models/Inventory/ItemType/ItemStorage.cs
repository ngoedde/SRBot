using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory.ItemType;

/// <summary>
/// For example: Elixir cubes, summoner cubes, etc.
/// </summary>
/// <param name="slot"></param>
/// <param name="item"></param>
/// <param name="rentInfo"></param>
public class ItemStorage(byte slot, RefObjItem item, ItemRentInfo rentInfo) : ItemContainer(slot, item, rentInfo)
{
    [Reactive] public uint Quantity { get; internal set; }
}