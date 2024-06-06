using System.Numerics;

namespace SRCore.Mathematics;

/// <summary>
/// Defines an infinite 3D ray with a starting position and a direction.
/// </summary>
public readonly struct Ray3
{
    public Vector3 Origin { get; }
    public Vector3 Direction { get; }
}