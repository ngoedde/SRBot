using SRCore.Components;
using SRCore.Models;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.MessageHandler.Agent.Auth;

internal class IdentificationRequest(AgentLogin agentLogin, Proxy proxy, AccountService accountService) : SRNetwork.MessageHandler
{
    public override PacketHandler Handler => Handle;

    public override ushort Opcode => 0x2001;

    
    public override ValueTask<bool> Handle(Session session, Packet packet)
    {
        try
        {
            if ((proxy.Context & ProxyContext.Client) != 0)
                return new ValueTask<bool>(true);
            
            var serviceName = packet.ReadString();

            if (serviceName != NetIdentity.AgentServer)
                return ValueTask.FromResult(true);

            if ((proxy.Context & ProxyContext.Agent) != 0)
                agentLogin.LoginToAgent();
             
            return ValueTask.FromResult(true);
        }
        catch (Exception e)
        {
            return ValueTask.FromResult(false);
        }
    }
}