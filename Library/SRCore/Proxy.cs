using System.Net;
using Serilog;
using SRNetwork;
using SRNetwork.Common;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore;

public class Proxy
{
    private ClientlessManager? _clientlessManager;
    
    public ProxyContext Context { get; private set; } = ProxyContext.None;
    public NetEngine Server { get; private set; } = new();
    public NetEngine Client { get; private set;  } = new();
    public Session? ClientSession { get; private set; }
    public Session? ServerSession { get; private set; }
    
    public Proxy()
    {
        Server.Connected += Proxy_OnServerConnected;
        Client.ClientConnected += Proxy_OnClientConnected;
    }

    public void Initialize(IEnumerable<SRNetwork.MessageHandler> packetHandlers, ClientlessManager clientlessManager)
    {
        _clientlessManager = clientlessManager;
        foreach (var packetHandler in packetHandlers)
        {
            Server.SetMsgHandler(packetHandler.Opcode, packetHandler.Handler);
            Client.SetMsgHandler(packetHandler.Opcode, packetHandler.Handler);
        }
    }
    
    public void SendToClient(Packet packet)
    {
        ClientSession?.Send(packet);
    }
    
    public void SendToServer(Packet packet)
    {
        ServerSession?.Send(packet);
    }
    
    public async Task StartClientProxy(ushort proxyPort)
    {
        Log.Information($"Starting client proxy on port {proxyPort}");
        
        Client.Start(proxyPort);
    }
    
    public async Task ConnectToGateway(EndPoint gatewayEndPoint)
    {
        Log.Information($"Connecting to gateway server at {gatewayEndPoint}");
        
        Context &= ~ProxyContext.Agent;
        Context |= ProxyContext.Gateway;
        
        await InitializeServerConnection(gatewayEndPoint);
    }
    
    public async Task ConnectToAgent(EndPoint agentEndPoint)
    {
        Log.Information($"Connecting to gateway server at {agentEndPoint}");
        
        Context |= ProxyContext.Agent;
        Context &= ~ProxyContext.Gateway;
        
        await InitializeServerConnection(agentEndPoint);
    }

    private async Task InitializeServerConnection(EndPoint serverEndPoint)
    {
        await Server.StopAsync();
        await Server.ConnectAsync(serverEndPoint);
    }
    
    private void Proxy_OnServerConnected(Session session)
    {
        Log.Information($"Connected to server: {session.RemoteEndPoint}");
        
        ServerSession = session;
        ServerSession.Disconnected += ServerSessionOnDisconnected;
        
        if ((Context & ProxyContext.Gateway) != 0 && ClientSession == null)
        {
            _clientlessManager.RequestPatchInfoAfterGatewayConnect();
        }
    }

    private void ServerSessionOnDisconnected(DisconnectReason reason)
    {
        ServerSession = null;
        Context &= ~ProxyContext.Gateway;
        Context &= ~ProxyContext.Agent;
        
        Log.Information("Disconnected from server.");
    }

    private void Proxy_OnClientConnected(Session session)
    {
        ClientSession = session;
        ClientSession.MessageReceived += Proxy_OnClientMessageReceived;
        ClientSession.Disconnected += Proxy_OnClientDisconnected;
        
        Context |= ProxyContext.Client;
        
        Log.Information($"Client connected: {session.RemoteEndPoint}");
    }

    private void Proxy_OnClientDisconnected(DisconnectReason reason)
    {
        Context &= ~ProxyContext.Client;

        ClientSession = null;
        
        Log.Information("Client disconnected.");
    }

    private void Proxy_OnClientMessageReceived(Packet packet)
    {
        ServerSession?.Send(packet);
    }

    public async Task ShutdownAsync()
    {
        await Client.StopAsync();
        await Server.StopAsync();
    }
}