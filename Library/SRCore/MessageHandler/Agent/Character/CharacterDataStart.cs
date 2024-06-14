using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Character;

internal class CharacterDataStart(Player player) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterDataStart;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            player.CharacterDataPacket = new Packet(AgentMsgId.CharacterData);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}