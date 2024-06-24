using System.Numerics;
using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class OrientedRegionPosition : RegionPosition
{
    [Reactive] public float Angle { get; internal set; }
    
    public void UpdateFromPacket(Packet packet)
    {
        RegionId = new RegionId(packet.ReadUShort());
        XOffset = packet.ReadFloat() * Constants.Scale;
        YOffset = packet.ReadFloat();
        ZOffset = packet.ReadFloat() * Constants.Scale;
        Angle = packet.ReadUShort() / 10_000f;
    }
    
    public float DistanceTo(OrientedRegionPosition other)
    {
        return Vector3.Distance(World, other.World);
    }

    public static OrientedRegionPosition FromRegionPosition(RegionPosition regionPosition, ushort angle = 0)
    {
        return new OrientedRegionPosition
        {
            RegionId = regionPosition.RegionId,
            XOffset = regionPosition.XOffset,
            YOffset = regionPosition.YOffset,
            ZOffset = regionPosition.ZOffset,
            Angle = angle
        };
    }

    public override string ToString()
    {
#if DEBUG
        return $"RegionId: {RegionId}, X: {XOffset}, Y: {YOffset}, Z: {ZOffset}, Angle: {Angle}";
#endif

        return $"X: {World.X}, Y: {World.Y}, Z: {World.Z}";
    }
}