using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.Character;

public class Attributes : ReactiveObject
{
    [Reactive] public uint PhysicalAttackMin { get; internal set; }
    [Reactive] public uint PhysicalAttackMax { get; internal set; }
    [Reactive] public uint MagicalAttackMin { get; internal set; }
    [Reactive] public uint MagicalAttackMax { get; internal set; }
    [Reactive] public ushort PhysicalDefence { get; internal set; }
    [Reactive] public ushort MagicalDefence { get; internal set; }
    [Reactive] public ushort HitRate { get; internal set; }
    [Reactive] public ushort ParryRate { get; internal set; }
    [Reactive] public uint MaxHealth { get; internal set; }
    [Reactive] public uint MaxMana { get; internal set; }
    [Reactive] public ushort Strength { get; internal set; }
    [Reactive] public ushort Intelligence { get; internal set; }

    public void UpdateFromPacket(Packet packet)
    {
        PhysicalAttackMin = packet.ReadUInt();
        PhysicalAttackMax = packet.ReadUInt();
        MagicalAttackMin = packet.ReadUInt();
        MagicalAttackMax = packet.ReadUInt();
        PhysicalDefence = packet.ReadUShort();
        MagicalDefence = packet.ReadUShort();
        HitRate = packet.ReadUShort();
        ParryRate = packet.ReadUShort();
        MaxHealth = packet.ReadUInt();
        MaxMana = packet.ReadUInt();
        Strength = packet.ReadUShort();
        Intelligence = packet.ReadUShort();
    }
}