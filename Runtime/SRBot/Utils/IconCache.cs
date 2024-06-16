using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Serilog;
using SRBot.Drawing;
using SRGame;
using SRGame.Client;
using SRPack.SRAdapter.Utils;

namespace SRBot.Utils;

public class IconCache(ClientFileSystem fileSystem)
{
    private object _lock = new();

    public const string IconRootDirectory = "icon";

    public Dictionary<string, Bitmap> Cache { get; } = new();

    private static bool locked = false;
    public async Task<Bitmap?> LoadIcon(string path)
    {
        path = Path.Combine(IconRootDirectory, PathUtils.Normalize(path));
        
        if (Cache.TryGetValue(path, out var icon))
        {
            return icon;
        }

        // var entry = Index.Find(x => x.Path == path);
        // if (entry == null)
        // {
        //     Log.Debug("Icon not found in index: {Path}", path);
        //     return null;
        // }

        try
        {
            // while (locked)
            //     await Task.Delay(10);

            locked = true;
            //var iconDDJStream =  await fileSystem.ReadFileBytes(AssetPack.Media, Path.Combine("icon", item.RefObjItem.AssocFileIcon)).ConfigureAwait(false);
            var iconDDJStream = await fileSystem.ReadFileBytes(AssetPack.Media, path).ConfigureAwait(false);
            var iconDDJ = iconDDJStream.ToArray();
            var iconDDS = iconDDJ[20..];
            var iconBMP = DDSImage.ToBitmap(iconDDS);

            AddIcon(path, iconBMP);

            return iconBMP;
        }
        catch (Exception e)
        {
            Log.Debug($"Error reading RefObjItem icon: {e.Message}");

            return null;
        }
        finally
        {
            locked = false;
        }
    }

    public bool TryGetIcon(string key, out Bitmap icon)
    {
        lock (_lock)
        {
            return Cache.TryGetValue(key, out icon);
        }
    }

    public void AddIcon(string key, Bitmap icon)
    {
        lock (_lock)
        {
            Cache[key] = icon;
        }
    }

    public void Clear()
    {
        Cache.Clear();
    }

    public void RemoveIcon(string key)
    {
        Cache.Remove(key);
    }

    public void RemoveIcons(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            Cache.Remove(key);
        }
    }

    public void RemoveIcons(params string[] keys)
    {
        foreach (var key in keys)
        {
            Cache.Remove(key);
        }
    }
}