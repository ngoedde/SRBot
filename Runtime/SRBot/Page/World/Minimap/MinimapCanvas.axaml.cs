using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using SRBot.Utils;
using SRCore.Mathematics;
using SRCore.Models;
using SRCore.Models.EntitySpawn;
using SRGame.Client;

namespace SRBot.Page.World.Minimap;

public partial class MinimapCanvas : UserControl
{
    private const int MinimapTileHeight = 256;
    private const int MinimapTileWidth = 256;
    
    public Vector3 MinimapTileSize => new (MinimapTileWidth, 0, MinimapTileHeight);
    public  Vector3 CanvasCenter => new((float) Width / 2, 0, (float) Height / 2);
    public MapTransform ViewportTransform { get; set; }
    
    private MinimapImageCache _imageCache;
    
    private Spawn _spawn;
    private Player _player;
    private bool _initialized;

    public MinimapCanvasConfig Config { get; set; } = new();
    
    public MinimapCanvas()
    {
        InitializeComponent();
    }
    
    public async Task Initialize(ClientFileSystem fileSystem, Spawn spawn, Player player)
    {
        _imageCache = new MinimapImageCache(fileSystem);
        _spawn = spawn;
        _player = player;
        _initialized = true;
    }

    public async Task Update(long delta)
    {
        // Get the current region id
        ViewportTransform = _player.Bionic != null
            ? new MapTransform(_player.Bionic.Position.RegionId, _player.Bionic.Position.Local)
            : new MapTransform(new RegionId(25000), new Vector3(0, 0, 0));
        
        //Warm up cache before rendering -> Since we are running rendering in UI thread,
        //this might otherwise cause parallel IO operations on the asset pack.
        await _imageCache.LoadMinimapIcons();
        foreach (var regionId in ViewportTransform.Region.Get9Neighbors())
            await _imageCache.LoadMinimapImage(regionId);
        
        Dispatcher.UIThread.InvokeAsync(this.InvalidateVisual);
    }
     
    public override void Render(DrawingContext context)
    {
        base.Render(context);
    
        if (!IsVisible || !_initialized)
            return;
        
        // Get the current and surrounding region ids
        var regionIds = ViewportTransform.Region.Get9Neighbors();
        
        try
        {
            foreach (var regionId in regionIds)
                //Parallel rendering of tiles. Can be awaited to render synchronously.
                DrawMinimapTile(regionId, context);

            foreach (var entity in _spawn.Entities)
                DrawEntity(entity, context);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void DrawMinimapTile(RegionId regionId, DrawingContext context)
    {
        if (!_imageCache.TryGetMinimapImage(regionId, out var image))
            return;
    
        // Calculate the position of the region relative to the ViewportTransform region
        var regionOffsetToSource = RegionId.Transform(ViewportTransform.Offset, ViewportTransform.Region, regionId);
                
        // Scale the position to fit within the minimap
        var minimapPosition = regionOffsetToSource / RegionId.Size * MinimapTileSize;
            
        // Calculate the draw position relative to the center of the canvas
        var drawPosition = CanvasCenter + minimapPosition;
        drawPosition = drawPosition with { Z = (float) Height / 2 - drawPosition.Z}; //flip X axis (draw top to bottom, not bottom to top);

        // Draw the image
        context.DrawImage(image, new Rect(drawPosition.X, drawPosition.Z, MinimapTileWidth, MinimapTileHeight));
                
        if (Config.DrawRegionBorder)
            context.DrawRectangle(Config.RegionBorderPen, new Rect(drawPosition.X, drawPosition.Z, MinimapTileWidth, MinimapTileHeight));
        
        if (Config.DrawRegionId)
        {
            var regionIdText = FormatText(regionId.ToString());

            // Draw the regionId in the center of the region
            context.DrawText(regionIdText, new Point(drawPosition.X + 8, drawPosition.Z + 8));
        }
    }

    private void DrawEntity<TEntity>(TEntity entity, DrawingContext context) where TEntity : Entity
    {
        if (!_imageCache.TryGetEntityImage(entity, out var image))
            return;

        // Calculate the position of the entity relative to the ViewportTransform region
        var entityOffsetToSource = RegionId.Transform(entity.Position.Local, entity.Position.RegionId, ViewportTransform.Region);

        // Scale the position to fit within the minimap
        var minimapPosition = entityOffsetToSource / RegionId.Size * MinimapTileSize;

        // Calculate the draw position relative to the center of the canvas
        var drawPosition = CanvasCenter + minimapPosition;
        // drawPosition = drawPosition with { Z = (float) Height / 2 - drawPosition.Z }; //flip X axis (draw top to bottom, not bottom to top);

        // Draw the image
        context.DrawImage(image, new Rect(drawPosition.X, drawPosition.Z, 8, 8));
    }
    
    private FormattedText FormatText(string text)
    {
        return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, Config.TextSizeEm, Config.TextColor);
    }
}