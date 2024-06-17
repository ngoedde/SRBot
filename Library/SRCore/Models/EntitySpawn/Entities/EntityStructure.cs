using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityStructure(RefObjChar refObjCommon) : EntityBionic(refObjCommon)
{
    [Reactive] public uint Health { get; set; }
    [Reactive] public uint RefEventStructId { get; set; }
    [Reactive] public ushort State { get; set; }
    
    public void ParseStructure(Packet packet)
    {
        Health = packet.ReadUInt();
        RefEventStructId = packet.ReadUInt();
        State = packet.ReadUShort();
    }
}