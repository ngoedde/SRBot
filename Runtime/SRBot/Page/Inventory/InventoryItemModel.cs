using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Serilog;
using SRBot.Drawing;
using SRCore.Models.Inventory;
using SRGame;
using SRGame.Client;

namespace SRBot.Page.Inventory;

public class InventoryItemModel(Item item, ClientFileSystem fileSystem) : ReactiveObject
{
    public Bitmap? Icon => GetIcon(fileSystem)?.GetAwaiter().GetResult();
    public Item Item { get; set; } = item;
    
    public async Task<Bitmap>? GetIcon(ClientFileSystem fileSystem)
    {
        try
        {
           //var iconDDJStream =  await fileSystem.ReadFileBytes(AssetPack.Media, Path.Combine("icon", item.RefObjItem.AssocFileIcon)).ConfigureAwait(false);
            var iconDDJStream =  await fileSystem.ReadFileBytes(AssetPack.Media, Path.Combine("icon", "icon_default.ddj")).ConfigureAwait(false);
            var iconDDJ = iconDDJStream.ToArray();
            var iconDDS = iconDDJ[20..];
            var iconBMP = DDSImage.ToBitmap(iconDDS);

            return iconBMP;
        }
        catch (Exception e)
        {
            Log.Debug($"Error reading RefObjItem icon: {e.Message}");
        }

        return null;
    }
}