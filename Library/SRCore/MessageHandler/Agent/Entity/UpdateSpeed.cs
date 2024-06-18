using SRCore.Models;
using SRCore.Models.EntitySpawn;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Entity;

internal class UpdateSpeed(Spawn spawn, Player player) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.EntityUpdateSpeed;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            var uniqueId = packet.ReadUInt();
            var walkSpeed = packet.ReadFloat();
            var runSpeed = packet.ReadFloat();

            if (uniqueId == player.UniqueId)
            {
                player.State.WalkSpeed = walkSpeed;
                player.State.RunSpeed = runSpeed;
            }
            if (spawn.TryGetEntity<EntityBionic>(uniqueId, out var entity))
            {
                entity.State.WalkSpeed = walkSpeed;
                entity.State.RunSpeed = runSpeed;
            }

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}