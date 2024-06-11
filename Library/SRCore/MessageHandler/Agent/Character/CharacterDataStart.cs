using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Character;

internal class CharacterDataStart(Models.Character character) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterDataStart;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            character.CharacterDataPacket = new Packet(AgentMsgId.CharacterData);
        
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