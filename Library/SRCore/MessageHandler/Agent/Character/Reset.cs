using SRCore.Models;

using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Character;

internal class Reset(Player player, Spawn spawn) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.GameReset;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}