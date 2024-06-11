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

            return ValueTask.FromResult(true);
        }
        catch (Exception)
        {
            return ValueTask.FromResult(false);
        }
        finally
        {
            OnHandled(session, packet);
        }
    }
}