using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRBot.Utils;
using SRCore.Models.Inventory;

namespace SRBot.Page.Inventory;

public class InventoryItemModel(Item item, IconCache iconCache) : ReactiveObject
{
    [Reactive] public Task<Bitmap?> Icon { get; set; } = iconCache.LoadIcon(item.RefObjItem.AssocFileIcon);
    public Item Item { get; set; } = item;
}