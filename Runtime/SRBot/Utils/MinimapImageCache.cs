using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Serilog;
using SRBot.Drawing;
using SRCore.Mathematics;
using SRCore.Models.EntitySpawn;
using SRCore.Models.EntitySpawn.Entities;
using SRGame;
using SRGame.Client;

namespace SRBot.Utils;

public class MinimapImageCache(ClientFileSystem fileSystem)
{
    public Dictionary<RegionId, Bitmap> MinimapImages { get; private set; } = new();
    
    public Dictionary<string, Bitmap> EntityIcons { get; private set; } = new();
    
    public bool TryGetMinimapImage(RegionId regionId, out Bitmap? bitmap)
    {
        return MinimapImages.TryGetValue(regionId, out bitmap);
    }

    public bool TryGetEntityImage<TEntity>(TEntity entity, out Bitmap? bitmap) where TEntity : Entity
    {
        var iconFileName = GetIconFileNameForEntityType(entity);
        
        return EntityIcons.TryGetValue(iconFileName, out bitmap);
    }

    public async Task LoadMinimapImage(RegionId regionId)
    {
        if (!fileSystem.IsInitialized)
            return;

        if (MinimapImages.TryGetValue(regionId, out _))
            return;
        
        var fileName = $"{regionId.X}x{regionId.Z}.ddj";
        var path = Path.Combine("minimap", fileName);
        
        try
        {           
            var iconDDJ = await fileSystem.ReadFile(AssetPack.Media, path).ConfigureAwait(false);
            var iconDDS = iconDDJ[20..];
            var iconBMP = DDSImage.ToBitmap(iconDDS);

            MinimapImages[regionId] = iconBMP;
        }
        catch (Exception e)
        {
            Log.Debug($"Error reading minimap {fileName} image: {e.Message}");
        }
    }
    
    public async Task LoadMinimapIcons(bool reload = false)
    {
        //Already loaded
        if (!reload && EntityIcons.Count > 0)
            return;
        
        var icons = new Dictionary<string, Bitmap>();
        var iconFiles = new[]
        {
            "mm_sign_character.ddj",
            "mm_sign_monster.ddj",
            "mm_sign_animal.ddj",
            "mm_sign_npc.ddj"
        };

        foreach (var iconFile in iconFiles)
        {
            var path = Path.Combine("interface", "minimap", iconFile);
            try
            {
                var iconDDJ = await fileSystem.ReadFile(AssetPack.Media, path).ConfigureAwait(false);
                var iconDDS = iconDDJ[20..];
                var iconBMP = DDSImage.ToBitmap(iconDDS);

                EntityIcons[iconFile] = iconBMP;
            }
            catch (Exception e)
            {
                Log.Debug($"Error reading entity {iconFile} image: {e.Message}");
            }
        }
    }
    
    private string GetIconFileNameForEntityType(Entity entity)
    {
        //ToDo: Advanced icons like party, unique, etc.
        switch (entity)
        {
            case EntityPlayer:
                return "mm_sign_character.ddj";
            case EntityMonster:
                return "mm_sign_monster.ddj";
            case EntityCos:
                return "mm_sign_animal.ddj";
            case EntityNpc:
                return "mm_sign_npc.ddj";
        }

        return "icon_default.ddj";
    }
}