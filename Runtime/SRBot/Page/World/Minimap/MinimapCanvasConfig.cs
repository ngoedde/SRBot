using Avalonia.Media;
using ReactiveUI.Fody.Helpers;
using SRCore.Config;

namespace SRBot.Page.World.Minimap;

public class MinimapCanvasConfig : ConfigElement
{
    [Reactive] public bool DrawRegionBorder { get; set; } = true;
    [Reactive] public bool DrawRegionId { get; set; } = true;

    [Reactive] public IImmutableSolidColorBrush TextColor { get; set; } = Brushes.Azure;
    [Reactive] public double TextSizeEm { get; set; } = 12;
    [Reactive] public Pen RegionBorderPen { get; set; } = new Pen(Brushes.Black, 1, new DashStyle(new double[] { 2, 2 }, 0));
    [Reactive] public bool DrawPlayers { get; set; } = true;
    [Reactive] public bool DrawEnemies { get; set; } = true;
    [Reactive] public bool DrawNpcs { get; set; } = true;
    [Reactive] public bool DrawCos { get; set; } = true;
    
}