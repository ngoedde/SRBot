using SRGame.Client;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory.ItemType;

public abstract class ItemContainer(byte slot, RefObjItem item, ItemRentInfo? rentInfo) : Item(slot, item, rentInfo)
{
    
}