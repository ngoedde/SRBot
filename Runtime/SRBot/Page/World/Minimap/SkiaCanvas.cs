#nullable disable
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace SRBot.Page.World.Minimap;

/// <summary>
///     A Xaml canvas control that can be drawn on using SkiaSharp drawing commands.
/// </summary>
/// <remarks>
///     See: https://github.com/mono/SkiaSharp/blob/main/source/SkiaSharp.Views/SkiaSharp.Views.UWP/SKXamlCanvas.cs.
/// </remarks>
public class SKXamlCanvas : Canvas
{
    private static readonly Vector Dpi = new(96, 96);

    private bool _IgnorePixelScaling;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SKXamlCanvas" /> class.
    /// </summary>
    public SKXamlCanvas()
    {
    }

    /// <summary>
    ///     Gets the current pixel size of the canvas.
    ///     Any scaling factor is already applied.
    /// </summary>
    public Size CanvasSize { get; private set; }

    /// <summary>
    ///     Gets the current render scaling applied to the control.
    /// </summary>
    public double Scale { get; private set; } = 1;

    /// <summary>
    ///     Gets or sets a value indicating whether the canvas's resolution and scale
    ///     will be automatically adjusted to match physical device pixels.
    /// </summary>
    public bool IgnorePixelScaling
    {
        get => _IgnorePixelScaling;
        set
        {
            _IgnorePixelScaling = value;
            Invalidate();
        }
    }

    /// <summary>
    ///     Event to externally paint the Skia surface (using the <see cref="SKCanvas" />).
    /// </summary>
    public event EventHandler<SKPaintSurfaceEventArgs> PaintSurface;

    /*
    private void OnDpiChanged(DisplayInformation sender, object args = null)
    {
        Dpi = sender.LogicalDpi / DpiBase;
        Invalidate();
    }
    */

    /// <summary>
    ///     Invalidates the canvas causing the surface to be repainted.
    ///     This will fire the <see cref="PaintSurface" /> event.
    /// </summary>
    public void Invalidate()
    {
        Dispatcher.UIThread.Post(() => RepaintSurface());
    }

    /// <summary>
    ///     Repaints the Skia surface and canvas.
    /// </summary>
    private void RepaintSurface()
    {
        if (!IsVisible) return;

        // Display scaling is important to consider here:
        // The bitmap itself should be sized to match physical device pixels.
        // This ensures it is never pixelated and renders properly to the display.
        // However, in several cases physical pixels do not match the logical pixels.
        // We also don't want to have to consider scaling in external code when calculating graphics.
        // To make this easiest, the layout scaling factor is calculated and then used
        // to find the size of the bitmap. This ensures it will match device pixels.
        // Then the canvas undoes this by setting a scale factor itself.
        // This means external code can use logical pixel size and the canvas will transform as needed.
        // Then the underlying bitmap is still at physical device pixel resolution.

        if (IgnorePixelScaling)
            Scale = 1;
        else
            Scale = LayoutHelper.GetLayoutScale(this);

        var pixelWidth = Convert.ToInt32(Bounds.Width * Scale);
        var pixelHeight = Convert.ToInt32(Bounds.Height * Scale);
        CanvasSize = new Size(pixelWidth, pixelHeight);

        // WriteableBitmap does not support zero-size dimensions
        // Therefore, to avoid a crash, exit here if size is zero
        if (pixelWidth == 0 ||
            pixelHeight == 0)
        {
            Background = null;
            return;
        }

        var bitmap = new WriteableBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            Dpi,
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        using (var framebuffer = bitmap.Lock())
        {
            var info = new SKImageInfo(
                framebuffer.Size.Width,
                framebuffer.Size.Height,
                framebuffer.Format.ToSkColorType(),
                SKAlphaType.Premul);

            var properties = new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal);

            // It is not too expensive to re-create the SKSurface on each re-paint.
            // See: https://groups.google.com/g/skia-discuss/c/3c10MvyaSug/m/UOr238asCgAJ
            //
            // When creating the SKSurface it is important to specify a pixel geometry
            // A defined pixel geometry is required for some anti-aliasing algorithms such as ClearType
            // Also see: https://github.com/AvaloniaUI/Avalonia/pull/9558
            using (var surface = SKSurface.Create(info, framebuffer.Address, framebuffer.RowBytes, properties))
            {
                if (!IgnorePixelScaling) surface.Canvas.Scale(Convert.ToSingle(Scale));

                OnPaintSurface(new SKPaintSurfaceEventArgs(surface, info, info));
            }

            properties.Dispose();
        }

        Background = new ImageBrush(bitmap)
        {
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top,
            Stretch = Stretch.Fill
        };
    }


    /// <summary>
    ///     Called when the canvas should repaint its surface.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        PaintSurface?.Invoke(this, e);
    }
    //
    // /// <inheritdoc/>
    // protected void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    // {
    //     base.OnPropertyChanged(change);
    //
    //     if (change.Property == IsVisibleProperty)
    //     {
    //         this.Invalidate();
    //     }
    //
    //     return;
    // }
}