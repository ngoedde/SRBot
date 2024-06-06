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
        return ValueTask.FromResult(shardListModel.TryParsePacket(session, packet));
    }
}