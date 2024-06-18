using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Character;

internal class CharacterDataEnd(Player player, Proxy proxy) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.CharacterDataEnd;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            player.ParsePacket(session, player.CharacterDataPacket.Lock());

            //Ready to play in case of clientless
            if ((proxy.Context & ProxyContext.Client) != 0) 
                return OnHandled(session, packet);
            
            var characterAckPacket = new Packet(AgentMsgId.ReadyToPlay);
            proxy.SendToServer(characterAckPacket);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}