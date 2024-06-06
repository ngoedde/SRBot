using System.Numerics;

namespace SRCore.Mathematics;

/// <summary>
/// Defines an infinite 2D ray with a starting position and a direction.
/// </summary>
public readonly struct Ray2
{
    public Vector2 Origin { get; }
    public Vector2 Direction { get; }
}