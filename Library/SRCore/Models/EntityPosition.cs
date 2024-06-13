using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class EntityPosition
{
    [Reactive] public RegionId RegionId { get; internal set; }
    [Reactive] public float X { get; internal set; }
    [Reactive] public float Y { get; internal set; }
    [Reactive] public float Z { get; internal set; }
    [Reactive] public ushort Angle { get; internal set; }
    
    public new static EntityPosition FromPacket(Packet packet)
    {
        var result = new EntityPosition
        {
            RegionId = new RegionId(packet.ReadUShort()),
            X = packet.ReadFloat(),
            Y = packet.ReadFloat(),
            Z = packet.ReadFloat(),
            Angle = packet.ReadUShort()
        };

        return result;
    }
}