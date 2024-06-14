using Serilog;
using SRCore.MessageHandler.Agent.Auth;
using SRCore.Models;
using SRGame.Client;
using SRNetwork;
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
)
{
    public void Initialize()
    {
        proxy.GatewayConnected += ProxyOnGatewayConnected;
        proxy.ClientConnected += ProxyOnClientConnected;

        proxy.GetHandler<Authentication>()!.Handled += OnAuthenticationHandled;

        patchInfo.PatchInfoUpdated += OnPatchInfoUpdated;
    }

    private void OnAuthenticationHandled(SRNetwork.MessageHandler handler, Session session, Packet packet)
    {
        if ((proxy.Context & ProxyContext.Client) != 0)
            return;
        
        var result = (MessageResult)packet.ReadByte();
        if (result != MessageResult.Success)
            return;

        packet.Reset();

        characterLobby.Request();
    }

    private async void ProxyOnClientConnected(Session clientSession)
    {
        if ((proxy.Context & ProxyContext.Agent) != 0)
            return;

        var endpoint = clientInfoManager.GetGatewayEndPoint();
        await proxy.ConnectToGateway(endpoint);
    }

    private void ProxyOnGatewayConnected(Session serverSession)
    {
        if ((proxy.Context & ProxyContext.Client) == 0)
            patchInfo.Request();
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