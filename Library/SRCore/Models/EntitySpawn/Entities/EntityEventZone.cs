using ReactiveUI.Fody.Helpers;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityEventZone() : Entity(null)
{
    [Reactive] public uint RefSkillId { get; internal set; }
    
    public void ParseEventZone(Packet packet)
    {
        packet.ReadUShort(); //TypeId?
        RefSkillId = packet.ReadUInt();
    }
}