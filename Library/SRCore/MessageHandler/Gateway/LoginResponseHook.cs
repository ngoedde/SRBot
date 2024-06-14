using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using SRCore.Models;
using SRNetwork.SilkroadSecurityApi;
using SRNetwork;


namespace SRCore.MessageHandler.Gateway;

internal class LoginResponseHook(IServiceProvider serviceProvider) : MessageHook
{
    public override PacketHook Hook => Handle;
    public override ushort Opcode => GatewayMsgId.LoginAck;

    private AgentLogin AgentLogin => serviceProvider.GetRequiredService<AgentLogin>();
    private Kernel Kernel => serviceProvider.GetRequiredService<Kernel>();
    private Proxy Proxy => serviceProvider.GetRequiredService<Proxy>();

    public override ValueTask<Packet> Handle(Session session, Packet packet)
    {
        var messageResult = (MessageResult)packet.ReadByte();
        if (messageResult != MessageResult.Success)
        {
            _ = Kernel.Panic(
                $"Login failed [reason={packet.ReadByte():X}]. Please check your credentials or try again later.",
                LogEventLevel.Error, (Proxy.Context & ProxyContext.Client) == 0);

            return ValueTask.FromResult(packet);
        }

        var token = packet.ReadUInt();
        var agentIp = packet.ReadString();
        var agentPort = packet.ReadUShort();

        AgentLogin.Token = token;
        AgentLogin.AgentServerIp = agentIp;
        AgentLogin.AgentServerPort = agentPort;

        //Only hook client login responses
        if ((Proxy.Context & ProxyContext.Client) == 0)
            return OnHooked(session, packet);

        var newPacket = new Packet(Opcode, packet.Encrypted, packet.Massive);
        newPacket.WriteByte(messageResult);
        newPacket.WriteUInt(token);
        newPacket.WriteString("127.0.0.1");
        newPacket.WriteUShort(AgentLogin.LocalPort);
        newPacket.Lock();

        return OnHooked(session, newPacket);
    }
}