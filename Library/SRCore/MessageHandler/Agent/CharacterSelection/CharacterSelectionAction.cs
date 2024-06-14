using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.CharacterSelection;

internal class CharacterSelectionResponse(CharacterLobby characterLobby) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterListActionResponse;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            characterLobby.ParsePacket(session, packet);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}