using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Movement(Position? destination = null) : ReactiveObject
{
    [Reactive] public Position? Destination { get; internal set; } = destination;
    [Reactive] public byte Type { get; internal set; }
    [Reactive] public ushort Angle { get; internal set; }
    
    public static Movement FromPacket(Packet packet)
    {
        var result = new Movement(Position.FromPacket(packet));
        
        result.Type = packet.ReadByte();
        result.Angle = packet.ReadUShort();
        
        if (packet.ReadBool())
        {
            result.Destination = Position.FromPacket(packet);
        }

        return result;
    }
}