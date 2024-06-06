using System.Numerics;

namespace SRCore.Mathematics;

/// <summary>
/// Represents a 2D line segment on a <see cref="Line2"/>
/// </summary>
public readonly struct Segment2
{
    public Vector2 Min { get; }
    public Vector2 Max { get; }
}