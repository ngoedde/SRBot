using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Character;

internal class CharacterDataEnd(Models.Player player) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterDataEnd;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
      
            player.ParsePacket(session, player.CharacterDataPacket.Lock());
        
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