using System.Numerics;
using System.Runtime.InteropServices;

namespace SRCore.Mathematics;

[StructLayout(LayoutKind.Auto)]
public readonly struct RectangleF : IEquatable<RectangleF>
{
    #region Fields

    private readonly Vector2 _min;
    private readonly Vector2 _max;

    #endregion Fields

    #region Properties

    public Vector2 Min => _min;
    public Vector2 Max => _max;

    public float X => _min.X;
    public float Y => _min.Y;
    public float Width => _max.X - _min.X;
    public float Height => _max.Y - _min.Y;
    public Vector2 Center => (_min + _max) * 0.5f;
    public Vector2 Size => _max - _min;

    #endregion Properties

    public RectangleF(Vector2 min, Vector2 max)
    {
        _min = min;
        _max = max;
    }

    public RectangleF(float x, float y, float width, float height)
    {
        _min = new Vector2(x, y);
        _max = new Vector2(x + width, y + height);
    }

    public bool Contains(Vector2 point)
    {
        return point.X >= _min.X && point.X <= _max.X
            && point.Y >= _min.Y && point.Y <= _max.Y;
    }

    public bool Contains(Vector3 point)
    {
        return point.X >= _min.X && point.X <= _max.X
            && point.Z >= _min.Y && point.Z <= _max.X;
    }

    public bool Intersects(RectangleF other) => other._min.X < _max.X && _min.X < other._max.X && other._min.Y < _max.Y && _min.Y < other._max.Y;

    public override bool Equals(object? obj) => obj is RectangleF f && this.Equals(f);

    public bool Equals(RectangleF other) => _min.Equals(other._min) && _max.Equals(other._max);

    public override int GetHashCode() => HashCode.Combine(_min, _max);

    public static RectangleF operator +(RectangleF left, RectangleF right) => new RectangleF(left._min + right._min, left._max + left._max);

    public static RectangleF operator -(RectangleF left, RectangleF right) => new RectangleF(left._min - right._min, left._max - left._max);

    public static RectangleF operator *(RectangleF left, float right) => new RectangleF(left._min * right, left._max * right);

    public static RectangleF operator /(RectangleF left, float right) => new RectangleF(left._min / right, left._max / right);

    public static bool operator ==(RectangleF left, RectangleF right) => left.Equals(right);

    public static bool operator !=(RectangleF left, RectangleF right) => !(left == right);

    public static RectangleF Transform(RectangleF value, Matrix4x4 matrix)
    {
        var min = Vector2.Transform(value._min, matrix);
        var max = Vector2.Transform(value._max, matrix);

        return new RectangleF(min, max);
    }
}