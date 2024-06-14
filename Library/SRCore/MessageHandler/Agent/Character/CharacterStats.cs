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