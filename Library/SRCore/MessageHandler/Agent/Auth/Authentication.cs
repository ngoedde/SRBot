using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Auth;

internal class Authentication(IServiceProvider serviceProvider) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.LoginActionAck;

    private Kernel Kernel => serviceProvider.GetRequiredService<Kernel>();
    private Proxy Proxy => serviceProvider.GetRequiredService<Proxy>();

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            var messageResult = (MessageResult)packet.ReadByte();
            if (messageResult != MessageResult.Success)
                _ = Kernel.Panic("Login to agent server failed", LogEventLevel.Error, (Proxy.Context & ProxyContext.Client) == 0);
            
            return OnHandled(session, packet);
        }
        catch (Exception e)
        {
            return ValueTask.FromResult(false);
        }
    }
}