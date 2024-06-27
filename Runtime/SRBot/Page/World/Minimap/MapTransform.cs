using System;
using System.Numerics;
using SRCore.Mathematics;

namespace SRBot.Page.World.Minimap;

public class MapTransform
{
    public Vector3 Offset;
    public RegionId Region;

    public MapTransform(RegionId region, Vector3 offset)
    {
        Region = region;
        Offset = offset;
    }

    public MapTransform(Vector3 worldPosition)
    {
        var regionX = (byte)(worldPosition.X / RegionId.Width);
        var regionZ = (byte)(worldPosition.Z / RegionId.Length);

        Region = new RegionId(regionX, regionZ);
        Offset = worldPosition - Region.Position;
    }

    public Vector3 Position => Region.Position + Offset;

    public void Break()
    {
        // Used to break a position which is exactly on 0 or 1920 after region transition.

        const float breakThreshold = 0.01f;

        if (Offset.X.IsApproximately(RegionId.Width))
            Offset.X = breakThreshold;
        else if (Offset.X.IsApproximately(0.0f))
            Offset.X = RegionId.Width - breakThreshold;

        if (Offset.Z.IsApproximately(RegionId.Length))
            Offset.Z = breakThreshold;
        else if (Offset.Z.IsApproximately(0.0f))
            Offset.Z = RegionId.Length - breakThreshold;
    }

    public void Normalize()
    {
        if (Region.IsDungeon)
            return;

        while (Offset.X >= RegionId.Width)
        {
            Region.X++;
            Offset.X -= RegionId.Width;
        }

        while (Offset.X < 0.0f)
        {
            Region.X--;
            Offset.X += RegionId.Width;
        }

        while (Offset.Z >= RegionId.Length)
        {
            Region.Z++;
            Offset.Z -= RegionId.Length;
        }

        while (Offset.Z < 0.0f)
        {
            Region.Z--;
            Offset.Z += RegionId.Length;
        }
    }

    public void NormalizeOnce()
    {
        if (Region.IsDungeon)
            return;

        if (Offset.X >= RegionId.Width)
        {
            Region.X++;
            Offset.X -= RegionId.Width;
        }

        if (Offset.X < 0.0f)
        {
            Region.X--;
            Offset.X += RegionId.Width;
        }

        if (Offset.Z >= RegionId.Length)
        {
            Region.Z++;
            Offset.Z -= RegionId.Length;
        }

        if (Offset.Z < 0.0f)
        {
            Region.Z--;
            Offset.Z += RegionId.Length;
        }
    }

    [Obsolete("Slower due to divisions, use Normalize instead")]
    public void NormalizeComplex()
    {
        if (Region.IsDungeon)
            return;

        // move x back into region space
        if (Offset.X >= RegionId.Width)
        {
            Region.X += (byte)(Offset.X / RegionId.Width);
            Offset.X %= RegionId.Width;
        }

        if (Offset.X < 0.0f)
        {
            Region.X += (byte)(Offset.X / RegionId.Width);
            Offset.X %= RegionId.Width;
        }

        // move z back into region space
        if (Offset.Z >= RegionId.Length)
        {
            Region.Z += (byte)(Offset.Z / RegionId.Length);
            Offset.Z %= RegionId.Length;
        }

        if (Offset.Z < 0.0f)
        {
            Region.Z += (byte)(Offset.Z / RegionId.Length);
            Offset.Z %= RegionId.Length;
        }
    }
    
    public Vector3 Transform(Vector3 point)
    {
        return RegionId.TransformPoint(this.Region, this.Region, point);
    }
}