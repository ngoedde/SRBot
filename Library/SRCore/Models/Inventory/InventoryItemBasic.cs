using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Inventory
{
    public class InventoryItemBasic(RefObjItem item) : ReactiveObject
    {
       [Reactive] public int RefObjId { get; internal set; } = item.Id;
       [Reactive] public RefObjItem Item { get; internal set; } = item;
       [Reactive] public byte OptLevel { get; internal set; }
    }
}