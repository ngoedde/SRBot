using System.Net;
using ReactiveUI.Fody.Helpers;
using Serilog;
using SRNetwork;
using SRNetwork.Common;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore;

public class Proxy
{
    public delegate void ClientConnectedEventHandler(Session clientSession);
    public delegate void GatewayConnectedEventHandler(Session serverSession);
    public delegate void AgentConnectedEventHandler(Session serverSession);

    public delegate void ClientDisconnectedEventHandler();
    public delegate void GatewayDisconnectedEventHandler();
    public delegate void AgentDisconnectedEventHandler();
    
    public event ClientConnectedEventHandler? ClientConnected;
    public event GatewayConnectedEventHandler? GatewayConnected;
    public event AgentConnectedEventHandler? AgentConnected;
    
    public event ClientDisconnectedEventHandler? ClientDisconnected;
    public event GatewayDisconnectedEventHandler? GatewayDisconnected;
    public event AgentDisconnectedEventHandler? AgentDisconnected;
    
    [Reactive] public ProxyContext Context { get; private set; } = ProxyContext.None;
    [Reactive] public NetEngine Server { get; private set; } = new();
    [Reactive] public NetEngine Client { get; private set;  } = new();
    [Reactive] public Session? ClientSession { get; private set; }
    [Reactive] public Session? ServerSession { get; private set; }
    
    public Proxy()
    {
        Server.Connected += Proxy_OnServerConnected;
        Client.ClientConnected += Proxy_OnClientConnected;
    }

    internal void Initialize(IEnumerable<SRNetwork.MessageHandler> packetHandlers)
    {
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

        if ((Context & ProxyContext.Gateway) != 0)
            OnGatewayConnected(session);
        else if ((Context & ProxyContext.Agent) != 0)
            OnAgentConnected(session);
    }

    private void ServerSessionOnDisconnected(DisconnectReason reason)
    {
        Log.Information("Disconnected from server.");
        ServerSession = null;
        
        if ((Context & ProxyContext.Agent) != 0) 
            OnAgentDisconnected();
        else if ((Context & ProxyContext.Gateway) != 0)
            OnGatewayDisconnected();
        
        Context &= ~ProxyContext.Gateway;
        Context &= ~ProxyContext.Agent;
    }

    private void Proxy_OnClientConnected(Session session)
    {
        Log.Information($"Client connected: {session.RemoteEndPoint}");

        ClientSession = session;
        ClientSession.MessageReceived += Proxy_OnClientMessageReceived;
        ClientSession.Disconnected += Proxy_OnClientDisconnected;
        
        Context |= ProxyContext.Client;
        
        OnClientConnected(session);
    }

    private void Proxy_OnClientDisconnected(DisconnectReason reason)
    {
        Log.Information("Client disconnected.");
        
        Context &= ~ProxyContext.Client;
        ClientSession = null;
        
        OnClientDisconnected();
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

    protected virtual void OnClientConnected(Session clientSession)
    {
        ClientConnected?.Invoke(clientSession);
    }

    protected virtual void OnGatewayConnected(Session serverSession)
    {
        GatewayConnected?.Invoke(serverSession);
    }

    protected virtual void OnAgentConnected(Session serverSession)
    {
        AgentConnected?.Invoke(serverSession);
    }

    protected virtual void OnClientDisconnected()
    {
        ClientDisconnected?.Invoke();
    }

    protected virtual void OnGatewayDisconnected()
    {
        GatewayDisconnected?.Invoke();
    }

    protected virtual void OnAgentDisconnected()
    {
        AgentDisconnected?.Invoke();
    }
}