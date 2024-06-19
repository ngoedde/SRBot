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
    public float XCoordinate => XOffset == 0 ? 0 : RegionId.IsDungeon ? XOffset / 10 : (RegionId.X - 135) * 192 + XOffset / 10;
    public float YCoordinate => ZOffset == 0 ? 0 : RegionId.IsDungeon ? ZOffset / 10 : (RegionId.Z - 92) * 192 + ZOffset / 10;

    public static RegionPosition FromPacket(Packet packet)
    {
        var result = new RegionPosition();

        var regionId = packet.ReadUShort();

        result.RegionId = new RegionId(regionId);
        if (regionId < short.MaxValue)
        {
            result.XOffset = packet.ReadUShort();
            result.YOffset = packet.ReadUShort();
            result.ZOffset = packet.ReadUShort();
        }
        else
        {
            result.XOffset = packet.ReadUInt();
            result.ZOffset = packet.ReadUInt();
            result.YOffset = packet.ReadUInt();
        }

        return result;
    }

    public float DistanceTo(RegionPosition other)
    {
        return Vector3.Distance(World, other.World);
    }

    public float DistanceTo(Vector3 other)
    {
        return Vector3.Distance(World, other);
    }
    
    public OrientedRegionPosition ToOrientedPosition(ushort angle = 0)
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
}