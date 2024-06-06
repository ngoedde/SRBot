using System.Numerics;

namespace SRGame.Mathematics;

public static class RegionIdExtensions
{
    public static Vector3 TransformPoint(this RegionId source, RegionId target, in Vector3 offset) => RegionId.TransformPoint(source, target, offset);

    public static Matrix4x4 TransformMatrix(this RegionId source, RegionId target, in Matrix4x4 matrix) => RegionId.TransformMatrix(source, target, matrix);
}