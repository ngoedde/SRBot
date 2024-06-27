using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Serilog;
using SkiaSharp;
using SRBot.Drawing;
using SRCore.Mathematics;
using SRGame;
using SRGame.Client;
using SRPack;

namespace SRBot.Utils;

public class MinimapImageCache(ClientFileSystem fileSystem)
{
    public Dictionary<RegionId, Bitmap> MinimapImages { get; } = new();

    public async Task<Bitmap?> GetOrAddMinimapImage(RegionId regionId)
    {
        if (!fileSystem.IsInitialized)
            return null;
        
        // if (minimapEntries == null)
        // {
        //     minimapEntries = await fileSystem.Media.GetEntriesAsync("minimap").ConfigureAwait(false);
        // }
        //
        if (MinimapImages.TryGetValue(regionId, out var bitmap))
            return bitmap;
        
        var fileName = $"{regionId.X}x{regionId.Z}.ddj";
        var path = Path.Combine("minimap", fileName);
        
        try
        {           
            var iconDDJStream = await fileSystem.ReadFile(AssetPack.Media, path).ConfigureAwait(false);
            var iconDDS = iconDDJStream[20..];
            var iconBMP = DDSImage.ToBitmap(iconDDS);

            MinimapImages[regionId] = iconBMP;
            
            return iconBMP;
        }
        catch (Exception e)
        {
            Log.Debug($"Error reading minimap {fileName} image: {e.Message}");

            return null;
        }
    }
}