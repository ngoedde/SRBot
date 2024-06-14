using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Movement(RegionPosition? destination = null) : ReactiveObject
{
    [Reactive] public RegionPosition? Destination { get; internal set; }

    [Reactive] public byte Type { get; internal set; }
    [Reactive] public MovementSourceType SourceType { get; internal set; }
    [Reactive] public ushort Angle { get; internal set; }

    internal static Movement FromPacket(Packet packet)
    {
        var result = new Movement();

        var hasDestination = packet.ReadBool();
        result.Type = packet.ReadByte();

        if (hasDestination)
        {
            result.Destination = RegionPosition.FromPacket(packet);
        }
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort();
        }


        return result;
    }
}