using ReactiveUI.Fody.Helpers;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn;

public class EntityBionic(RefObjChar refObjChar) : Entity(refObjChar)
{
    [Reactive] public RefObjChar RefObjChar { get; internal set; }
    [Reactive] public Movement Movement { get; internal set; } = new Movement();
    [Reactive] public State State { get; internal set; } = new State();

    public void ParseBionic(Packet packet, EntityManager entityManager)
    {
        Movement = Movement.FromPacketNoSource(this, packet);
        State = State.FromPacket(packet, entityManager);
    }
}