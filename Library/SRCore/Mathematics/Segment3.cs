using System.Numerics;

namespace SRCore.Mathematics;

/// <summary>
/// Represents a 3D line segment on a <see cref="Line3"/>
/// </summary>
public readonly struct Segment3
{
    public Vector3 Min { get; }
    public Vector3 Max { get; }
}