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
            var result = characterLobby.TryParsePacket(session, packet);

            return ValueTask.FromResult(result);
        }
        catch (Exception e)
        {
            return ValueTask.FromResult(false);
        }
    }
}