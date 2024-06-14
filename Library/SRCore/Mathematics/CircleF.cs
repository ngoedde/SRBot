using System.Numerics;
using System.Runtime.InteropServices;

namespace SRCore.Mathematics;

[StructLayout(LayoutKind.Auto)]
public readonly struct CircleF
{
    private readonly Vector2 _position;
    private readonly float _radius;

    public Vector2 Position => _position;
    public float Radius => _radius;

    public CircleF(Vector2 position, float radius)
    {
        _position = position;
        _radius = radius;
    }

    public bool Contains(Vector2 point) => MathHelper.Distance2DSqrt(_position, point) <= (_radius * _radius);

    public bool Contains(Vector3 point) => MathHelper.Distance2DSqrt(_position, point) <= (_radius * _radius);

    public bool Intersects(Vector2 min, Vector2 max, out Vector2 point) =>
        IntersectionHelper.SegmentCircle(min, max, _position, _radius, out point);
}