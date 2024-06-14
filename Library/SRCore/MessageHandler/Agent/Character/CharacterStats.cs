using SRCore.Models.Character;

namespace SRCore.MessageHandler.Agent.Character;

using Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

internal class CharacterStats(Player player) : MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterStatsUpdate;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            player.Attributes = Attributes.FromPacket(packet);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}