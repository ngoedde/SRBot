using System.Numerics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class RegionPosition : ReactiveObject
{
    [Reactive] public RegionId RegionId { get; internal set; }
    [Reactive] public float XOffset { get; internal set; }
    [Reactive] public float YOffset { get; internal set; }
    [Reactive] public float ZOffset { get; internal set; }

    public Vector3 Local => new Vector3(XOffset, YOffset, ZOffset);
    public Vector3 World => Vector3.Transform(Local, RegionId.LocalToWorld);
    public Mathematics.RegionPosition ToRegionPosition() => new(RegionId, new Vector3(XOffset, YOffset, ZOffset));

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

    public float DistanceTo(EntityPosition other)
    {
        return Vector3.Distance(World, other.World);
    }

    public EntityPosition ToEntityPosition(ushort angle = 0)
    {
        return new EntityPosition
        {
            RegionId = RegionId,
            X = XOffset,
            Y = YOffset,
            Z = ZOffset,
            Angle = angle
        };
    }
}