using Microsoft.Extensions.DependencyInjection;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models
{
    internal class AgentLogin(IServiceProvider serviceProvider) : GameModel(serviceProvider)
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string AgentServerIp { get; set; } = string.Empty;

        public ushort AgentServerPort { get; set; }

        public byte ContentId { get; set; }

        public ushort ShardId { get; set; }

        public uint Token { get; set; }

        public ushort LocalPort => _proxy.LocalPort;

        private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();

        public void LoginToAgent()
        {
            var agentHandshakePacket = new Packet(AgentMsgId.LoginActionReq, true);
            agentHandshakePacket.WriteUInt(Token);
            agentHandshakePacket.WriteString(Username);
            agentHandshakePacket.WriteString(Password);
            agentHandshakePacket.WriteByte(ContentId);
            agentHandshakePacket.WriteByteArray(new byte[6]);

            _proxy.SendToServer(agentHandshakePacket);
        }
    }
}