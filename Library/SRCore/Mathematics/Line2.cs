using System.Numerics;

namespace SRCore.Mathematics;

/// <summary>
/// Represents an infinite 2D line through two points.
/// </summary>
public readonly struct Line2
{
    public Vector2 Min { get; }
    public Vector2 Max { get; }
}