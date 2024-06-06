using System.Numerics;

namespace SRCore.Mathematics;

/// <summary>
/// Represents an infinite 3D line through two points.
/// </summary>
public readonly struct Line3
{
    public Vector3 Min { get; }
    public Vector3 Max { get; }
}