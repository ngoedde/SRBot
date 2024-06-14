using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Gateway;

internal class ShardListMessageHandler(ShardList shardListModel) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;
    public override ushort Opcode => GatewayMsgId.ShardInfoAck;

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            shardListModel.ParsePacket(session, packet);

            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return OnHandled(session, packet, e);
        }
    }
}