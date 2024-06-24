using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore;

public class KernelMetrics : ReactiveObject
{
    [Reactive] public int FPS { get; internal set; } = 0;
    [Reactive] public int TargetFPS { get; set; } = 60;
    [Reactive] public double FrameTime { get; internal set; }
    [Reactive] public double TargetFrameTime { get; internal set; }
    
    [Reactive] public double IdleTime { get; internal set; }
    
    [Reactive] public long TotalFrames { get; internal set; }
}