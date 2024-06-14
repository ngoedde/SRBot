#if UNITY_STANDALONE || UNITY_EDITOR
using MathF = UnityEngine.Mathf;

#else

using MathF = System.MathF;
#endif
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SRCore.Mathematics;

public static class MathHelper
{
    public const float PiOverFour = MathF.PI * 0.25f; // 45°
    public const float PiOverTwo = MathF.PI * 0.5f; // 90°
    public const float PI = MathF.PI; // 180°
    public const float TwoPI = 2 * MathF.PI; // 360°
    public const float Tau = 2 * MathF.PI; // 360°
    public const float Epsilon = 1e-6f; // 0.000001

    public const float DegreeToRad = PI / 180.0f;
    public const float RadToDegree = 180.0f / PI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Square(float value) => value * value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float value) => MathF.Sqrt(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float value) => MathF.Sin(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float value) => MathF.Cos(value);

    [Obsolete("Use Orientation or Cross2D instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float VectorSign2D(Vector2 a, Vector2 b, Vector2 p) => Cross2D(a - p, b - p);

    [Obsolete("Use Orientation or Cross2D instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float VectorSign2D(Vector3 a, Vector3 b, Vector3 p) => Cross2D(a - p, b - p);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross2D(Vector2 left, Vector2 right) => (left.X * right.Y) - (left.Y * right.X);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross2D(Vector3 left, Vector3 right) => (left.X * right.Z) - (left.Z * right.X);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot2D(Vector2 left, Vector2 right) => (left.X * right.X) + (left.Y * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot2D(Vector3 left, Vector3 right) => (left.X * right.X) + (left.Z * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2DSqrt(Vector2 left, Vector2 right) =>
        ((left.X - right.X) * (left.X - right.X)) + ((left.Y - right.Y) * (left.Y - right.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2DSqrt(Vector2 left, Vector3 right)
    {
        var dx = left.X - right.X;
        var dz = left.Y - right.Z;
        return (dx * dx) + (dz * dz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2DSqrt(Vector3 left, Vector2 right)
    {
        var dx = left.X - right.X;
        var dz = left.Z - right.Y;
        return (dx * dx) + (dz * dz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2DSqrt(Vector3 left, Vector3 right)
    {
        var dx = left.X - right.X;
        var dz = left.Z - right.Z;
        return (dx * dx) + (dz * dz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2DSqrt(RegionPosition left, RegionPosition right)
    {
        var dx = ((left.RegionId.X - right.RegionId.X) * RegionId.Width) + (left.Offset.X - right.Offset.X);
        var dz = ((left.RegionId.Z - right.RegionId.Z) * RegionId.Length) + (left.Offset.Z - right.Offset.Z);

        return (dx * dx) + (dz * dz);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2D(Vector2 left, Vector2 right) => MathF.Sqrt(Distance2DSqrt(left, right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2D(Vector3 left, Vector3 right) => MathF.Sqrt(Distance2DSqrt(left, right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance2D(RegionPosition left, RegionPosition right) =>
        MathF.Sqrt(Distance2DSqrt(left, right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float min, float max, float t) => ((1.0f - t) * min) + (t * max);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LerpFast(float min, float max, float t) => min + (t * (max - min));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseLerp(float min, float max, float t)
    {
        if (MathF.Abs(t) < Epsilon)
            return min;

        return (t - min) / (max - min);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(this float value, float min, float max)
    {
        Debug.Assert(max > min, $"min({min:0.000}) exceeded max({max:0.000}) value");
        return value < min ? min : value > max ? max : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min, int max)
    {
        Debug.Assert(max > min, $"min({min}) exceeded max({max}) value");
        return value < min ? min : value > max ? max : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp01(float value) => Clamp(value, 0.0f, 1.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximately(this float a, float b, float threshold = Epsilon) =>
        MathF.Abs(a - b) < threshold;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximatelyZero(this float value, float threshold = Epsilon) => MathF.Abs(value) < threshold;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this Vector2 p, Vector2 min, Vector2 max)
    {
        // Based on https://stackoverflow.com/a/328193

        // If the cross product of (p - min) and (max - min) is 0 then the points are aligned.
        if (!Cross2D(p - min, max - min).IsApproximatelyZero())
            return false;

        // If the dot product of (p - min) and (max - min) is positive and less then
        // the squared magnitude of (max - min) then p is between min and max.
        var dot = Dot2D(p - min, max - min);
        return dot >= 0 && dot <= Distance2DSqrt(min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this Vector3 p, Vector3 min, Vector3 max)
    {
        // Based on https://stackoverflow.com/a/328193

        // If the cross product of (p - min) and (max - min) is 0 then the points are aligned.
        if (!Cross2D(p - min, max - min).IsApproximatelyZero())
            return false;

        // If the dot product of (p - min) and (max - min) is positive and less then
        // the squared magnitude of (max - min) then p is between min and max.
        var dot = Dot2D(p - min, max - min);
        return dot >= 0 && dot <= Distance2DSqrt(min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VectorOrientation Orientation(Vector2 a, Vector2 b, Vector2 p)
    {
        var cross = Cross2D(a - p, b - p);
        if (cross.IsApproximatelyZero())
            return VectorOrientation.Collinear;

        return cross > 0 ? VectorOrientation.Left : VectorOrientation.Right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VectorOrientation Orientation(Vector3 a, Vector3 b, Vector3 p)
    {
        var cross = Cross2D(a - p, b - p);
        if (cross.IsApproximatelyZero())
            return VectorOrientation.Collinear;

        return cross > 0 ? VectorOrientation.Left : VectorOrientation.Right;
    }
}