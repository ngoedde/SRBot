using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.Character;

public class Job : ReactiveObject
{
    [Reactive] public string Name { get; internal set; } = string.Empty;
    [Reactive] public byte Type { get; internal set; }
    [Reactive] public byte Level { get; internal set; }
    [Reactive] public uint Experience { get; internal set; }
    [Reactive] public uint Contribution { get; internal set; }
    [Reactive] public uint Reward { get; internal set; }

    internal static Job FromPacket(Packet packet)
    {
        var result = new Job
        {
            Name = packet.ReadString(),
            Type = packet.ReadByte(),
            Level = packet.ReadByte(),
            Experience = packet.ReadUInt(),
            Contribution = packet.ReadUInt(),
            Reward = packet.ReadUInt()
        };

        return result;
    }
}