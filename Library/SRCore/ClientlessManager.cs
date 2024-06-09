using Serilog;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRGame.Client;
using SRNetwork;
using SRNetwork.Common;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore;

/// <summary>
/// Automatically handles clientless/client mode.
/// </summary>
internal class ClientlessManager(PatchInfo patchInfo, ShardList shardList, ProfileService profileService, Proxy proxy, ClientInfoManager clientInfoManager, AgentLogin agentLogin)
{
    private Profile ActiveProfile => profileService.ActiveProfile;

    public void Initialize()
    {
        patchInfo.PatchInfoUpdated += OnPatchInfoUpdated;

        proxy.GatewayConnected += ProxyOnGatewayConnected;
        proxy.ClientConnected += ProxyOnClientConnected;
        //proxy.AgentConnected += ProxyOnAgentConnected;
    }

    private void ProxyOnAgentConnected(Session serverSession)
    {
        throw new NotImplementedException();
    }

    private async void ProxyOnClientConnected(Session clientSession)
    {
        if (ActiveProfile.Clientless)
            return;

        if ((proxy.Context & ProxyContext.Agent) != 0)
        {
            return;
        }

        var endpoint = clientInfoManager.GetGatewayEndPoint();
        await proxy.ConnectToGateway(endpoint);
    }

    private async void ServerSessionOnMessageReceived(Packet packet)
    {
        if (packet.Opcode != GatewayMsgId.LoginAck)
            return;

        var result = (MessageResult)packet.ReadByte();
        if (result != MessageResult.Success)
            return;

        await proxy.ConnectToAgent(NetHelper.ToIPEndPoint(agentLogin.AgentServerIp, agentLogin.AgentServerPort), agentLogin.Token);
    }

    private void ProxyOnGatewayConnected(Session serverSession)
    {
        if (ActiveProfile.Clientless)
            patchInfo.Request();

        serverSession.MessageReceived += ServerSessionOnMessageReceived;
    }
    
    private async void OnPatchInfoUpdated(Session session, PatchInfo patchInfo)
    {
        if (patchInfo.PatchRequired)
        {
            Log.Fatal("!!! Game update available !!! Please run Silkroad.exe to update the client.");
            
            await session.DisconnectAsync();
            
            return;
        }

        if (ActiveProfile.Clientless)
            shardList.Request();
    }

}