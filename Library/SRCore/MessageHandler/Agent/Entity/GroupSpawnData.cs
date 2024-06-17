using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Entity;

internal class GroupSpawnData(Spawn spawn) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.GroupSpawnData;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            spawn.GroupSpawnPacket.WriteByteArray(packet.GetBytes());

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}