using System.Numerics;

namespace SRCore.Mathematics;

public static class VectorExtensions
{
    public static Vector3 Flatten(this Vector3 value) => new(value.X, 0, value.Z);

    public static Vector3 ToVector3(this Vector2 value) => new(value.X, 0, value.Y);

    public static Vector2 ToVector2(this Vector3 value) => new(value.X, value.Z);
}