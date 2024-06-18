using ReactiveUI.Fody.Helpers;
using SRGame;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityPlayer(RefObjChar refObjChar) : EntityCharacter(refObjChar)
{
    [Reactive] public override string Name { get; internal set; }
    [Reactive] public byte JobType { get; internal set; }
    [Reactive] public byte JobLevel { get; internal set; }
    [Reactive] public PvPState PvPState { get; internal set; }
    [Reactive] public bool IsRiding { get; internal set; }
    [Reactive] public bool InCombat { get; internal set; }
    [Reactive] public uint TransportUniqueId { get; internal set; }
    [Reactive] public ScrollMode ScrollMode { get; internal set; }
    [Reactive] public byte InteractMode { get; internal set; }
    [Reactive] public string GuildName { get; internal set; } = string.Empty;
    [Reactive] public uint GuildId { get; internal set; }
    [Reactive] public string GuildNickName { get; internal set; }
    [Reactive] public uint GuildLastCrestRev { get; internal set; }
    [Reactive] public uint GuildUnionId { get; internal set; }
    [Reactive] public uint GuildUnionLastCrestRev { get; internal set; }
    [Reactive] public bool GuildIsFriendly { get; internal set; }
    [Reactive] public byte GuildSiegeAuthority { get; internal set; }
    [Reactive] public byte EquipmentCooldown { get; internal set; }
    [Reactive] public byte PKFlag { get; internal set; }

    public void ParsePlayer(Packet packet)
    {
        Name = packet.ReadString();
        JobType = packet.ReadByte();
        JobLevel = packet.ReadByte();
        PvPState = (PvPState)packet.ReadByte();
        IsRiding = packet.ReadBool();
        InCombat = packet.ReadBool();
        if (IsRiding)
            TransportUniqueId = packet.ReadUInt();
        ScrollMode = (ScrollMode)packet.ReadByte();
        InteractMode = packet.ReadByte();
        packet.ReadByte(); //UUnknonwn
        GuildName = packet.ReadString();

        //ToDo: JobEquipment check!
        GuildId = packet.ReadUInt();
        GuildNickName = packet.ReadString();
        GuildLastCrestRev = packet.ReadUInt();
        GuildUnionId = packet.ReadUInt();
        GuildUnionLastCrestRev = packet.ReadUInt();
        GuildIsFriendly = packet.ReadBool();
        GuildSiegeAuthority = packet.ReadByte();

        if (InteractMode == 4) //Stall mode
        {
            packet.ReadString(); //Stall name
            packet.ReadUInt(); //Decoration
        }

        EquipmentCooldown = packet.ReadByte();
        PKFlag = packet.ReadByte();
    }
}