using System.Net;
using SRCore.Models;
using SRNetwork.SilkroadSecurityApi;
using SRNetwork;


namespace SRCore.MessageHandler.Gateway
{
    internal class LoginResponseHook(AgentLogin agentLogin) : MessageHook
    {
        public override PacketHook Hook => Handle;
        public override ushort Opcode => GatewayMsgId.LoginAck;

        public override ValueTask<Packet> Handle(Session session, Packet packet)
        {
            var messageResult = (MessageResult)packet.ReadByte();
            if (messageResult != MessageResult.Success)
                return ValueTask.FromResult(packet);

            var token = packet.ReadUInt();
            var agentIp = packet.ReadString();
            var agentPort = packet.ReadUShort();

            agentLogin.Token = token;
            agentLogin.AgentServerIp = agentIp;
            agentLogin.AgentServerPort = agentPort;

            var newPacket = new Packet(Opcode, packet.Encrypted, packet.Massive);
            newPacket.WriteByte(messageResult);
            newPacket.WriteUInt(token);
            newPacket.WriteString("127.0.0.1");
            newPacket.WriteUShort(agentLogin.LocalPort);

            return ValueTask.FromResult(newPacket);
        }
    }
}
