using System.Numerics;
using System.Runtime.InteropServices;

namespace SRCore.Mathematics;

[StructLayout(LayoutKind.Auto)]
public readonly struct TriangleF
{
    private readonly Vector3 _v0;
    private readonly Vector3 _v1;
    private readonly Vector3 _v2;

    public Vector3 V0 => _v0;
    public Vector3 V1 => _v1;
    public Vector3 V2 => _v2;

    public Vector3 Center => (_v0 + _v1 + _v2) / 3.0f;

    public TriangleF(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        v0 = v0;
        _v1 = v1;
        _v2 = v2;
    }

    public bool TryFindHeight(ref Vector3 vPos)
    {
        //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html

        //Based on Barycentric coordinates
        float denominator = ((_v1.Z - _v2.Z) * (_v0.X - _v2.X)) + ((_v2.X - _v1.X) * (_v0.Z - _v2.Z));

        float a = (((_v1.Z - _v2.Z) * (vPos.X - _v2.X)) + ((_v2.X - _v1.X) * (vPos.Z - _v2.Z))) / denominator;
        float b = (((_v2.Z - _v0.Z) * (vPos.X - _v2.X)) + ((_v0.X - _v2.X) * (vPos.Z - _v2.Z))) / denominator;
        float c = 1 - a - b;

        vPos.Y = ((a * _v0.Y) + (b * _v1.Y) + (c * _v2.Y)) / (a + b + c);
        //return a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f; // point is within the triangle
        return a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f; // point can be on border
    }

    public bool Contains(Vector2 value) => this.Contains(value.ToVector3());

    public bool Contains(Vector3 value) => IntersectionHelper.IsPointOnTriangle(_v0, _v1, _v2, value);

    public static TriangleF Transform(TriangleF value, Matrix4x4 matrix)
    {
        var p1 = Vector3.Transform(value._v0, matrix);
        var p2 = Vector3.Transform(value._v1, matrix);
        var p3 = Vector3.Transform(value._v2, matrix);

        return new TriangleF(p1, p2, p3);
    }
}