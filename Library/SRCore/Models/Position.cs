using System.Numerics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Position : ReactiveObject
{
    [Reactive] public RegionId RegionId { get; internal set; }
    [Reactive] public uint XOffset { get; internal set; }
    [Reactive] public uint YOffset { get; internal set; }
    [Reactive] public uint ZOffset { get; internal set; }
    
    public RegionPosition ToRegionPosition() => new(RegionId, new Vector3(XOffset, YOffset, ZOffset));

    public static Position FromPacket(Packet packet)
    {
        var result = new Position();
        
        var regionId = packet.ReadUShort();
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
}