using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using SRBot.Utils;
using SRCore.Mathematics;
using SRCore.Models;
using SRGame.Client;

namespace SRBot.Page.World.Minimap;

public partial class MinimapCanvas : UserControl
{
    private const int UpdateInterval = 100;
    
    private MinimapImageCache _imageCache;
    public MapTransform ViewportTransform;

    private Spawn _spawn;
    private Player _player;
    
    private bool _initialized;
    
    public MinimapCanvas()
    {
        InitializeComponent();
    }
    
    public void Initialize(ClientFileSystem fileSystem, Spawn spawn, Player player)
    {
        _imageCache = new MinimapImageCache(fileSystem);
        _spawn = spawn;
        _player = player;
        _initialized = true;
    }

    public async Task Update(long delta)
    {
        if (_isPainting)
            return;

        // Get the current region id
        ViewportTransform = _player.Bionic != null
            ? new MapTransform(_player.Bionic.Position.RegionId, _player.Bionic.Position.Local)
            : new MapTransform(new RegionId(RegionId.Center), new Vector3(0, 0, 0));
        
        //Warm up cache before rendering -> Since we are running rendering in UI thread,
        //this might otherwise cause parallel IO operations on the asset pack.
        foreach (var regionId in ViewportTransform.Region.Get9Neighbors())
            _= await _imageCache.GetOrAddMinimapImage(regionId);
        
        Dispatcher.UIThread.InvokeAsync(this.InvalidateVisual);
    }
    
    private bool _isPainting;
     
    public override async void Render(DrawingContext context)
    {
        base.Render(context);

        if (!IsVisible || !_initialized)
            return;

        _isPainting = true;

        // Get the current and surrounding region ids
        var regionIds = ViewportTransform.Region.Get9Neighbors();

        int imageWidth = 256, imageHeight = 256;
        var regionSize = new Vector3(RegionId.Width, 0, RegionId.Length);
        var minimapSize = new Vector3(imageWidth, 0, imageHeight);
        var canvasCenter = new Vector3((float) Width / 2, 0, (float) Height / 2);

        try
        {
            // Iterate over the region ids
            foreach (RegionId regionId in regionIds)
            {
                // Fetch the corresponding image from the MinimapImageCache
                var image = await _imageCache.GetOrAddMinimapImage(regionId);
                if (image == null)
                    continue;

                // Calculate the position of the region relative to the ViewportTransform region
                var regionOffsetToSource = RegionId.Transform(ViewportTransform.Offset, ViewportTransform.Region, regionId);

                // Scale the position to fit within the minimap
                var minimapPosition = regionOffsetToSource / regionSize * minimapSize;

                // Calculate the draw position relative to the center of the canvas
                var drawPosition = canvasCenter + minimapPosition;
                drawPosition = drawPosition with { Z = (float) Height - drawPosition.Z }; //flip Y axis
                
                // Draw the image
                context.DrawImage(image, new Rect(drawPosition.X, drawPosition.Z, imageWidth, imageHeight));
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        _isPainting = false;
    }
}