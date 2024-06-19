using SRCore.Models;
using SRCore.Models.EntitySpawn;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Entity;

internal class UpdateMovement(Spawn spawn, Player player) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.EntityUpdateMovement;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            var uniqueId = packet.ReadUInt();

                     
            var bionic = uniqueId == player.Bionic.UniqueId ? player.Bionic : null;
            if (bionic == null && !spawn.TryGetEntity(uniqueId, out bionic))
                return OnHandled(session, packet);

            bionic!.Movement = Movement.FromPacketWithSource(bionic, packet);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}