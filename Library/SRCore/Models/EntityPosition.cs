using ReactiveUI.Fody.Helpers;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class EntityPosition : Position
{
    [Reactive] public ushort Angle { get; internal set; }
    
    public new static EntityPosition FromPacket(Packet packet)
    {
        var result = new EntityPosition();
        
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
        
        result.Angle = packet.ReadUShort();

        return result;
    }
}