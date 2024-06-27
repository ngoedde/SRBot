using System;
using System.Diagnostics;
using System.Numerics;
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

    public void Update(long delta)
    {
        if (_isPainting)
            return;
        
        Dispatcher.UIThread.InvokeAsync(this.InvalidateVisual).Wait();
    }
    
    private bool _isPainting;
     
    public override async void Render(DrawingContext context)
    {
        base.Render(context);

        if (!IsVisible || !_initialized) 
            return;
        
        _isPainting = true;
        
        ViewportTransform = _player.Bionic == null ?
            new MapTransform(new RegionId(25000), new Vector3(0, 0, 0)) 
            : new MapTransform(_player.Bionic.Position.Local);
        
        try
        {
            // var transX = ((float) Width - ViewportTransform.Offset.X);
            // var transY = ((float) Height - ViewportTransform.Offset.Z);

            var bitmap = await _imageCache.GetOrAddMinimapImage(ViewportTransform.Region);
            if (bitmap == null)
                return;
            
            // Iterate over the region ids
            foreach (RegionId regionId in ViewportTransform.Region.Get9Neighbors())
            {
                // Fetch the corresponding image from the MinimapImageCache
                var image = await _imageCache.GetOrAddMinimapImage(regionId);

                if (image == null)
                    continue;
                
                // Use the ViewportTransform property to transform the coordinates of the region to the viewport's coordinate system
                var transformedPosition = regionId.TransformPoint(ViewportTransform.Region, ViewportTransform.Offset);
                // Draw the image at the transformed coordinates
                context.DrawImage(image, new Rect( transformedPosition.X, 1920 - transformedPosition.Z, 256, 256));
            }
        
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        _isPainting = false;
    }
}