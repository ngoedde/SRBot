using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Gateway;

internal class PatchInfoMessageHandler(PatchInfo patchInfo) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;
    public override ushort Opcode => GatewayMsgId.PatchInfoAck;
    
    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        return ValueTask.FromResult(patchInfo.TryParsePacket(session, packet));
    }
}