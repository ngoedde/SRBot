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
    public NetEngine Server { get; } = new();
    public NetEngine Client { get; } = new();
    [Reactive] public Session? ClientSession { get; private set; }
    [Reactive] public Session? ServerSession { get; private set; }

    [Reactive] public ushort LocalPort { get; private set; }

    private uint _agentToken = 0;

    public Proxy()
    {
        Server.Connected += Proxy_OnServerConnected;
        Client.ClientConnected += Proxy_OnClientConnected;
    }

    internal void Initialize(IEnumerable<SRNetwork.MessageHandler> packetHandlers, IEnumerable<SRNetwork.MessageHook>  packetHooks)
    {
        foreach (var packetHandler in packetHandlers)
        {
            Server.SetMsgHandler(packetHandler.Opcode, packetHandler.Handler);
            Client.SetMsgHandler(packetHandler.Opcode, packetHandler.Handler);
        }

        foreach (var packetHandler in packetHooks)
        {
            Server.SetMsgHook(packetHandler.Opcode, packetHandler.Hook);
            Client.SetMsgHook(packetHandler.Opcode, packetHandler.Hook);
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
        LocalPort = proxyPort;
        Client.Start(proxyPort);

        Log.Information($"Started client proxy on 0.0.0.0:{proxyPort}");
    }

    public async Task ConnectToGateway(EndPoint gatewayEndPoint)
    {
        Log.Information($"Connecting to gateway server at {gatewayEndPoint}");

        Context &= ~ProxyContext.Agent;
        Context |= ProxyContext.Gateway;

        await InitializeServerConnection(gatewayEndPoint);
    }

    public async Task ConnectToAgent(EndPoint agentEndPoint, uint token)
    {
        Log.Information($"Connecting to agent server at {agentEndPoint}");

        Context |= ProxyContext.Agent;
        Context &= ~ProxyContext.Gateway;

        _agentToken = token;

        await InitializeServerConnection(agentEndPoint);
    }

    private async Task InitializeServerConnection(EndPoint serverEndPoint)
    {
        if (ServerSession != null)
            await ServerSession.DisconnectAsync();

        //await Server.StopAsync();
        await Server.ConnectAsync(serverEndPoint);
    }

    private void Proxy_OnServerConnected(Session session)
    {
        Log.Information($"Connected to server: {session.RemoteEndPoint}");

        ServerSession = session;
        ServerSession.Disconnected += ServerSessionOnDisconnected;
        ServerSession.MessageReceived += ServerSessionOnMessageReceived;

        if ((Context & ProxyContext.Gateway) != 0)
            OnGatewayConnected(session);
        else if ((Context & ProxyContext.Agent) != 0)
            OnAgentConnected(session);
    }

    private void ServerSessionOnMessageReceived(Packet packet)
    {
        Log.Debug($"Received from server: {packet}");

        ClientSession?.Send(packet);
    }

    private void ServerSessionOnDisconnected(DisconnectReason reason)
    {
        Log.Information("Disconnected from server.");
        ServerSession = null;

        if ((Context & ProxyContext.Agent) != 0)
        {
            Context &= ~ProxyContext.Agent;
            OnAgentDisconnected();
        }
        else if ((Context & ProxyContext.Gateway) != 0)
        {
            Context &= ~ProxyContext.Gateway;
            OnGatewayDisconnected();
        }
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
        Log.Debug($"Received from client: {packet}");

        if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000 || packet.Opcode == 0x2001)
            return;

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