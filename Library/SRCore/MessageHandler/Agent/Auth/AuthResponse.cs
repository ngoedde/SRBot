using Microsoft.Extensions.DependencyInjection;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Auth;

internal class AuthResponse(IServiceProvider serviceProvider) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => AgentMsgId.LoginActionAck;
    
    private Kernel Kernel => serviceProvider.GetRequiredService<Kernel>();

    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            var messageResult = (MessageResult)packet.ReadByte();

            if (messageResult != MessageResult.Success)
            {
                _ = Kernel.TriggerInterrupt("Login to agent server failed");

                return ValueTask.FromResult(true);
            }
            
            return ValueTask.FromResult(true);
        }
        catch (Exception e)
        {
            return ValueTask.FromResult(false);
        }
    }
}