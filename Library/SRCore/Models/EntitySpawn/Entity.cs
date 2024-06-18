using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn;

public abstract class Entity(RefObjCommon? refObjCommon) : ReactiveObject
{
    [Reactive] public int RefObjId { get; internal set; } = refObjCommon?.Id ?? int.MaxValue;
    
    [Reactive] public uint UniqueId { get; internal set; }
    
    [Reactive] public EntityPosition Position { get; internal set; } = new EntityPosition();

    [Reactive] public virtual string Name { get; internal set; } = refObjCommon?.Name ?? string.Empty;

    public void ParseEntity(Packet packet)
    {
        UniqueId = packet.ReadUInt();
        Position = EntityPosition.FromPacket(packet);
    }
}