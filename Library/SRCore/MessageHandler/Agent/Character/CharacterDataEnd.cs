using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Character;

internal class CharacterDataEnd(Models.Character character) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterDataEnd;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
      
            character.ParsePacket(session, character.CharacterDataPacket.Lock());
        
            return ValueTask.FromResult(true);
        }
        catch (Exception e)
        {
            return ValueTask.FromResult(false);
        }
        finally
        {
            OnHandled(session, packet);
        }
    }
}