using System.Numerics;
using System.Runtime.InteropServices;

namespace SRCore.Mathematics;

[StructLayout(LayoutKind.Auto)]
public readonly struct BoundingBoxF : IEquatable<BoundingBoxF>
{
    #region Fields

    private readonly Vector3 _min;
    private readonly Vector3 _max;

    #endregion Fields

    #region Properties

    public float Width => _max.X - _min.X;
    public float Height => _max.Y - _min.Y;
    public float Length => _max.Z - _min.Z;
    public Vector3 Center => (_max + _min) * 0.5f;

    public Vector3 Min => _min;
    public Vector3 Max => _max;

    public Vector3 Size => new Vector3(this.Width, this.Height, this.Length);

    #endregion Properties

    public BoundingBoxF(in Vector3 min, in Vector3 max)
    {
        _min = min;
        _max = max;
    }

    public bool Contains(in Vector3 p)
    {
        return p.X >= _min.X && p.X <= _max.X
                             && p.Y >= _min.Y && p.Y <= _max.Y
                             && p.Z >= _min.Z && p.Z <= _max.Z;
    }

    public override bool Equals(object? obj) => obj is BoundingBoxF f && this.Equals(f);

    public bool Equals(BoundingBoxF other) => _min.Equals(other._min) && _max.Equals(other._max);

    public override int GetHashCode() => HashCode.Combine(_min, _max);

    public static bool operator ==(BoundingBoxF left, BoundingBoxF right) => left.Equals(right);

    public static bool operator !=(BoundingBoxF left, BoundingBoxF right) => !(left == right);

    public static BoundingBoxF operator +(BoundingBoxF left, BoundingBoxF right) =>
        new BoundingBoxF(left._min + right._min, left._max + right._max);

    public static BoundingBoxF operator -(BoundingBoxF left, BoundingBoxF right) =>
        new BoundingBoxF(left._min + right._min, left._max + right._max);

    public static BoundingBoxF operator *(BoundingBoxF left, float right) =>
        new BoundingBoxF(left._min * right, left._max * right);

    public static BoundingBoxF operator /(BoundingBoxF left, float right) =>
        new BoundingBoxF(left._min / right, left._max / right);
}