using System.Numerics;
using System.Runtime.CompilerServices;

namespace SRCore.Mathematics;

public static class IntersectionHelper
{
    public static bool LineLine(Vector2 min0, Vector2 max0, Vector2 min1, Vector2 max1, out Vector2 point)
    {
        var denominator = MathHelper.Cross2D(max0 - min0, max1 - min1);
        if (denominator == 0.0f)
        {
            point = Vector2.Zero;
            return false;
        }

        var time0 = MathHelper.Cross2D(min1 - min0, max1 - min1) / denominator;
        //var time1 = MathHelper.Cross2D(min1 - min0, max0 - min0) / denominator;

        point = Vector2.Lerp(min0, max0, time0);

        return true;
    }

    public static bool SegmentSegment(Vector2 min0, Vector2 max0, Vector2 min1, Vector2 max1, out Vector2 point)
    {
        var denominator = MathHelper.Cross2D(max0 - min0, max1 - min1);
        if (denominator == 0.0f)
        {
            point = Vector2.Zero;
            return false;
        }

        var time0 = MathHelper.Cross2D(min1 - min0, max1 - min1) / denominator;
        var time1 = MathHelper.Cross2D(min1 - min0, max0 - min0) / denominator;

        if (time0 is < 0.0f or > 1.0f || time1 is < 0.0f or > 1.0f)
        {
            point = Vector2.Zero;
            return false;
        }

        point = Vector2.Lerp(min0, max0, time0);
        return true;
    }

    public static bool SegmentSegment(Vector3 min0, Vector3 max0, Vector3 min1, Vector3 max1, out Vector3 point)
    {
        var denominator = MathHelper.Cross2D(max0 - min0, max1 - min1);
        if (denominator == 0.0f)
        {
            point = Vector3.Zero;
            return false;
        }

        var time0 = MathHelper.Cross2D(min1 - min0, max1 - min1) / denominator;
        var time1 = MathHelper.Cross2D(min1 - min0, max0 - min0) / denominator;

        if (time0 is < 0.0f or > 1.0f || time1 is < 0.0f or > 1.0f)
        {
            point = Vector3.Zero;
            return false;
        }

        point = Vector3.Lerp(min1, max1, time1);
        return true;
    }

    public static bool SegmentSegment01(Vector2 min0, Vector2 max0, Vector2 min1, Vector2 max1, out Vector2 point)
    {
        var denominator = MathHelper.Cross2D(max0 - min0, max1 - min1);
        if (denominator == 0.0f)
        {
            point = Vector2.Zero;
            return false;
        }

        //var time0 = MathHelper.Cross2D(min1 - min0, max1 - min1) / denominator;
        var time1 = MathHelper.Cross2D(min1 - min0, max0 - min0) / denominator;
        point = Vector2.Lerp(min1, max1, MathHelper.Clamp01(time1));
        return true;
    }

    public static bool SegmentSegment01(Vector3 min0, Vector3 max0, Vector3 min1, Vector3 max1, out Vector3 point)
    {
        var denominator = MathHelper.Cross2D(max0 - min0, max1 - min1);
        if (denominator == 0.0f)
        {
            point = Vector3.Zero;
            return false;
        }

        //var time0 = MathHelper.Cross2D(min1 - min0, max1 - min1) / denominator;
        var time1 = MathHelper.Cross2D(min1 - min0, max0 - min0) / denominator;
        point = Vector3.Lerp(min1, max1, MathHelper.Clamp01(time1));
        return true;
    }

    public static int LineCircle(Vector2 min, Vector2 max, Vector2 p, float radius, out Vector2 hit0, out Vector2 hit1)
    {
        // http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        var d = max - min;
        var f = min - p;

        var a = MathHelper.Dot2D(d, d);
        var b = 2 * MathHelper.Dot2D(f, d);
        var c = MathHelper.Dot2D(f, f) - (radius * radius);

        var discriminantSquared = (b * b) - (4.0f * a * c);
        if ((a <= float.Epsilon) || discriminantSquared < 0)
        {
            hit0 = default;
            hit1 = default;
            return 0;
        }

        if (discriminantSquared == 0)
        {
            // One solution.
            var t = -b / (2.0f * a);
            hit0 = min + (d * t);
            hit1 = default;
            return 1;
        }

        var discriminant = MathHelper.Sqrt(discriminantSquared);

        // Two solutions.
        var t0 = ((-b + discriminant) / (2.0f * a));
        hit0 = min + (d * t0);

        var t1 = ((-b - discriminant) / (2.0f * a));
        hit1 = min + (d * t1);
        return 2;
    }

    public static int LineSphere(Vector3 min, Vector3 max, Vector3 p, float radius, out Vector3 hit0, out Vector3 hit1)
    {
        // http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        var d = max - min;
        var f = min - p;

        var a = Vector3.Dot(d, d);
        var b = 2 * Vector3.Dot(f, d);
        var c = Vector3.Dot(f, f) - (radius * radius);

        var discriminantSquared = (b * b) - (4.0f * a * c);
        if ((a <= float.Epsilon) || discriminantSquared < 0)
        {
            // No real solutions.
            hit0 = default;
            hit1 = default;
            return 0;
        }

        if (discriminantSquared == 0)
        {
            // One solution.
            var t = -b / (2.0f * a);
            hit0 = min + (d * t);
            hit1 = default;
            return 1;
        }

        var discriminant = MathHelper.Sqrt(discriminantSquared);

        // Two solutions.
        var t0 = ((-b + discriminant) / (2.0f * a));
        hit0 = min + (d * t0);

        var t1 = ((-b - discriminant) / (2.0f * a));
        hit1 = min + (d * t1);
        return 2;
    }

    public static bool SegmentCircle(Vector2 min, Vector2 max, Vector2 p, float radius, out Vector2 hit)
    {
        var result = LineCircle(min, max, p, radius, out Vector2 hit0, out Vector2 hit1);
        switch (result)
        {
            case 1:
                hit = hit0;
                return hit.IsBetween(min, max);
            case 2:
                // Keep the closest hit to min.
                hit = MathHelper.Distance2DSqrt(min, hit0) < MathHelper.Distance2DSqrt(min, hit1) ? hit0 : hit1;
                return hit.IsBetween(min, max);
            default:
                hit = default;
                return false;
        }
    }

    public static bool SegmentSphere(Vector3 min, Vector3 max, Vector3 p, float radius, out Vector3 hit)
    {
        var result = LineSphere(min, max, p, radius, out Vector3 hit0, out Vector3 hit1);
        switch (result)
        {
            case 1:
                hit = hit0;
                return hit.IsBetween(min, max);
            case 2:
                // Keep the closest hit to min.
                hit = Vector3.DistanceSquared(min, hit0) < Vector3.DistanceSquared(min, hit1) ? hit0 : hit1;
                return hit.IsBetween(min, max);
            default:
                hit = default;
                return false;
        }
    }

    public static bool IsPointInTriangle(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 p)
    {
        var isLeft0 = MathHelper.Cross2D(v0 - p, v1 - p) < 0.0f;
        var isLeft1 = MathHelper.Cross2D(v1 - p, v2 - p) < 0.0f;
        var isLeft2 = MathHelper.Cross2D(v2 - p, v0 - p) < 0.0f;

        return isLeft0 == isLeft1 && isLeft1 == isLeft2;
    }

    public static bool IsPointInTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 p)
    {
        var isLeft0 = MathHelper.Cross2D(v0 - p, v1 - p) < 0.0f;
        var isLeft1 = MathHelper.Cross2D(v1 - p, v2 - p) < 0.0f;
        var isLeft2 = MathHelper.Cross2D(v2 - p, v0 - p) < 0.0f;

        return isLeft0 == isLeft1 && isLeft1 == isLeft2;
    }

    public static bool IsPointOnTriangle(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 p)
    {
        var isLeft0 = MathHelper.Cross2D(v0 - p, v1 - p) <= 0.0f;
        var isLeft1 = MathHelper.Cross2D(v1 - p, v2 - p) <= 0.0f;
        var isLeft2 = MathHelper.Cross2D(v2 - p, v0 - p) <= 0.0f;

        return isLeft0 == isLeft1 && isLeft1 == isLeft2;
    }

    public static bool IsPointOnTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 p)
    {
        var isLeft0 = MathHelper.Cross2D(v0 - p, v1 - p) <= 0.0f;
        var isLeft1 = MathHelper.Cross2D(v1 - p, v2 - p) <= 0.0f;
        var isLeft2 = MathHelper.Cross2D(v2 - p, v0 - p) <= 0.0f;

        return isLeft0 == isLeft1 && isLeft1 == isLeft2;
    }

    public static bool TryFindHeightInTriangle(Vector3 v0, Vector3 v1, Vector3 v2, ref Vector3 p)
    {
        if (!TryCalculateBaryCentricCoordinates(v0, v1, v2, p, out var baryCentric))
            return false;

        p.Y = ((baryCentric.X * v0.Y) + (baryCentric.Y * v1.Y) + (baryCentric.Z * v2.Y)) /
              (baryCentric.X + baryCentric.Y + baryCentric.Z);

        return baryCentric.X is > 0f and < 1f &&
               baryCentric.Y is > 0f and < 1f &&
               baryCentric.Z is > 0f and < 1f;
    }

    public static bool TryFindHeightOnTriangle(Vector3 v0, Vector3 v1, Vector3 v2, ref Vector3 p)
    {
        if (!TryCalculateBaryCentricCoordinates(v0, v1, v2, p, out var baryCentric))
            return false;

        p.Y = ((baryCentric.X * v0.Y) + (baryCentric.Y * v1.Y) + (baryCentric.Z * v2.Y)) /
              (baryCentric.X + baryCentric.Y + baryCentric.Z);

        return baryCentric.X is >= 0f and <= 1f &&
               baryCentric.Y is >= 0f and <= 1f &&
               baryCentric.Z is >= 0f and <= 1f;
    }

    private static bool TryCalculateBaryCentricCoordinates(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 p,
        out Vector3 baryCentric)
    {
        Unsafe.SkipInit(out baryCentric);

        //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
        //Based on Barycentric coordinates
        var denominator = ((v1.Z - v2.Z) * (v0.X - v2.X)) + ((v2.X - v1.X) * (v0.Z - v2.Z));
        if (denominator == 0.0f)
            return false;

        baryCentric.X = (((v1.Z - v2.Z) * (p.X - v2.X)) + ((v2.X - v1.X) * (p.Z - v2.Z))) / denominator;
        baryCentric.Y = (((v2.Z - v0.Z) * (p.X - v2.X)) + ((v0.X - v2.X) * (p.Z - v2.Z))) / denominator;
        baryCentric.Z = 1.0f - baryCentric.X - baryCentric.Y;
        return true;
    }

    public static bool LineRectangle(Vector2 v0, Vector2 v1, RectangleF rect, out Vector2 point)
    {
        // Useful resources:
        // * Math for Game Developers - Bullet Collision (Vector/AABB Intersection):
        // * https://www.youtube.com/watch?v=USjbg5QXk3g

        var tMin = 0.0f;
        var tMax = 1.0f;

        var minX = (rect.Min.X - v0.X) / (v1.X - v0.X);
        var maxX = (rect.Max.X - v0.X) / (v1.X - v0.X);

        if (maxX < minX)
            (minX, maxX) = (maxX, minX);

        if (maxX < tMin || minX > tMax)
        {
            point = Vector2.Zero;
            return false;
        }

        tMin = MathF.Max(minX, tMin);
        tMax = MathF.Min(maxX, tMax);

        if (tMin > tMax)
        {
            point = Vector2.Zero;
            return false;
        }

        var minY = (rect.Min.Y - v0.Y) / (v1.Y - v0.Y);
        var maxY = (rect.Max.Y - v0.Y) / (v1.Y - v0.Y);

        if (maxY < minY)
            (minY, maxY) = (maxY, minY);

        if (maxY < tMin || minY > tMax)
        {
            point = Vector2.Zero;
            return false;
        }

        tMin = MathF.Max(minY, tMin);
        tMax = MathF.Min(maxY, tMax);

        if (tMin > tMax)
        {
            point = Vector2.Zero;
            return false;
        }

        var b = (v1 - v0);
        point = v0 + (b * tMin);
        return true;
    }
}