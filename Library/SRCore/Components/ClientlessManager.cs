using Serilog;
using SRCore.Models;
using SRGame.Client;
using SRNetwork;
using SRNetwork.Common;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Components;

/// <summary>
/// Automatically handles clientless/client mode.
/// </summary>
internal class ClientlessManager(
    PatchInfo patchInfo,
    ShardList shardList,
    Proxy proxy,
    ClientInfoManager clientInfoManager,
    AgentLogin agentLogin,
    CharacterLobby characterLobby
) {
    
    public void Initialize()
    {
        proxy.GatewayConnected += ProxyOnGatewayConnected;
        proxy.ClientConnected += ProxyOnClientConnected;
        proxy.AgentConnected += ProxyOnAgentConnected;
        patchInfo.PatchInfoUpdated += OnPatchInfoUpdated;
    }

    private void ProxyOnAgentConnected(Session serverSession)
    {
        serverSession.MessageReceived += ServerSessionOnMessageReceived;
    }

    private async void ProxyOnClientConnected(Session clientSession)
    {
        if ((proxy.Context & ProxyContext.Client) == 0)
            return;

        if ((proxy.Context & ProxyContext.Agent) != 0)
        {
            return;
        }
        
        //For clientless operation -> Connect to gateway
        var endpoint = clientInfoManager.GetGatewayEndPoint();
        await proxy.ConnectToGateway(endpoint);
    }
    
    private void ProxyOnGatewayConnected(Session serverSession)
    {
        if ((proxy.Context & ProxyContext.Client) == 0)
            patchInfo.Request();

        serverSession.MessageReceived += ServerSessionOnMessageReceived;
    }

    private async void ServerSessionOnMessageReceived(Packet packet)
    {
        MessageResult result;
        
        //For clientless operation -> Request character list after login
        if (packet.Opcode == AgentMsgId.LoginActionAck && (proxy.Context & ProxyContext.Client) == 0)
        {
            result = (MessageResult)packet.ReadByte();
            if (result != MessageResult.Success)
                return;
            
            characterLobby.Request();
        }
    }


    private async void OnPatchInfoUpdated(Session session, PatchInfo patchInfo)
    {
        if (patchInfo.PatchRequired)
        {
            Log.Fatal("!!! Game update available !!! Please run Silkroad.exe to update the client.");
            
            await session.DisconnectAsync();
            
            return;
        }

        if ((proxy.Context & ProxyContext.Client) == 0)
            shardList.Request();
    }

}