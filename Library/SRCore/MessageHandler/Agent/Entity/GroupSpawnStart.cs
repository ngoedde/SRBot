using SRCore.Models;
using SRCore.Models.EntitySpawn;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Entity;

internal class GroupSpawnStart(Spawn spawn) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.GroupSpawnStart;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            spawn.GroupSpawnType = (GroupSpawnType)packet.ReadByte();
            spawn.GroupSpawnCount = packet.ReadUShort();
            spawn.GroupSpawnPacket = new Packet(AgentMsgId.GroupSpawnStart);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}