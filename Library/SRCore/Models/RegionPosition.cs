using System.Numerics;
using ReactiveUI;
using SRCore.Mathematics;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class RegionPosition : ReactiveObject
{
    private float _x;
    private float _y;
    private float _z;
    private RegionId _regionId = RegionId.Invalid;
    
    public RegionId RegionId
    {
        get => _regionId;
        internal set
        {
            this.RaiseAndSetIfChanged(ref _regionId, value);
            this.RaisePropertyChanged(nameof(XCoordinate));
            this.RaisePropertyChanged(nameof(YCoordinate));
            this.RaisePropertyChanged(nameof(World));
            this.RaisePropertyChanged(nameof(Local));
        }
    }

    public float XOffset
    {
        get => _x;
        internal set
        {
            this.RaiseAndSetIfChanged(ref _x, value);
            this.RaisePropertyChanged(nameof(XCoordinate));
            this.RaisePropertyChanged(nameof(World));
            this.RaisePropertyChanged(nameof(Local));
        }
    }

    public float YOffset
    {
        get => _y;
        internal set
        {
            this.RaiseAndSetIfChanged(ref _y, value);
            this.RaisePropertyChanged(nameof(World));
            this.RaisePropertyChanged(nameof(Local));
        }
    }

    public float ZOffset
    {
        get => _z;
        internal set
        {
            this.RaiseAndSetIfChanged(ref _z, value);
            this.RaisePropertyChanged(nameof(World));
            this.RaisePropertyChanged(nameof(Local));
            this.RaisePropertyChanged(nameof(YCoordinate));
        }
    }

    public Vector3 Local => new Vector3(XOffset, YOffset, ZOffset);
    public Vector3 World => Vector3.Transform(Local, RegionId.LocalToWorld);

    public float XCoordinate => XOffset == 0 ? 0 : RegionId.IsDungeon ? XOffset : (RegionId.X - 135) * 192 + XOffset;
    public float YCoordinate => ZOffset == 0 ? 0 : RegionId.IsDungeon ? ZOffset : (RegionId.Z - 92) * 192 + ZOffset;

    public static RegionPosition FromPacket(Packet packet)
    {
        var result = new RegionPosition();

        var regionId = packet.ReadUShort();

        result.RegionId = new RegionId(regionId);
        if (regionId < short.MaxValue)
        {
            result.XOffset = packet.ReadShort();
            result.YOffset = packet.ReadShort();
            result.ZOffset = packet.ReadShort();
        }
        else
        {
            result.XOffset = packet.ReadInt();
            result.YOffset = packet.ReadInt();
            result.ZOffset = packet.ReadInt();
        }

        return result;
    }

    public OrientedRegionPosition ToOrientedPosition(float angle = 0)
    {
        return new OrientedRegionPosition
        {
            RegionId = RegionId,
            XOffset = XOffset,
            YOffset = YOffset,
            ZOffset = ZOffset,
            Angle = angle
        };
    }
    public void TieBreak()
    {
        // Used to break a position which is exactly on 0 or 1920 after region transition.
        const float breakThreshold = 0.01f;

        // X
        if (XOffset.IsApproximately(RegionId.Width))
        {
            XOffset = MathHelper.Epsilon;
            _regionId.X++;
        }
        else if (XOffset.IsApproximatelyZero(0.0f))
        {
            XOffset = RegionId.Width - breakThreshold;
            _regionId.X--;
        }

        // Z
        if (ZOffset.IsApproximately(RegionId.Length))
        {
            ZOffset = MathHelper.Epsilon;
            _regionId.Z++;
        }
        else if (ZOffset.IsApproximatelyZero(0.0f))
        {
            ZOffset = RegionId.Length - breakThreshold;
            _regionId.Z--;
        }
    }

    public float DistanceTo(RegionPosition other)
    {
        var destination = RegionId.Transform(other.Local, other.RegionId, _regionId);

        return Vector3.Distance(destination, this.Local);
    }

    public void Normalize()
    {
        if (_regionId.IsDungeon)
            return;

        if (XOffset > RegionId.Width)
        {
            _regionId.X += (byte)(XOffset / RegionId.Width);
            XOffset %= RegionId.Width;
        }
        else if (XOffset < 0.0f)
        {
            _regionId.X += (byte)(XOffset / RegionId.Width);
            XOffset = RegionId.Width + (XOffset % RegionId.Width);
        }

        if (ZOffset > RegionId.Length)
        {
            _regionId.Z += (byte)(ZOffset / RegionId.Length);
            ZOffset %= RegionId.Length;
        }
        else if (ZOffset < 0.0f)
        {
            _regionId.Z += (byte)(ZOffset / RegionId.Length);
            ZOffset = RegionId.Length + (ZOffset % RegionId.Length);
        }
    }
}