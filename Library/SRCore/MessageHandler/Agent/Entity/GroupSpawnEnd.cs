using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Entity;

internal class GroupSpawnEnd(Spawn spawn) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.GroupSpawnEnd;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            spawn.ParsePacket(session, spawn.GroupSpawnPacket.Lock());
            
            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}