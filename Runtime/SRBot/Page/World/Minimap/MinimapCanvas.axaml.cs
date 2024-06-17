using Avalonia.Controls;
using SkiaSharp;

namespace SRBot.Page.World.Minimap;

public partial class MinimapCanvas : UserControl
{
    public MapTransform ViewportTransform;
    
    public MinimapCanvas()
    {
        InitializeComponent();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        var transX = ((float) Width - ViewportTransform.Offset.X);
        var transY = ((float) Height - ViewportTransform.Offset.Z);

        canvas.DrawText($"X: {ViewportTransform.Position.X}", 10, 10, new SKPaint { Color = SKColors.White });
        canvas.DrawText($"Y: {ViewportTransform.Position.Y}", 10, 10, new SKPaint { Color = SKColors.White });
    }
}