using System.Net;
using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using Serilog;
using SRCore.Models;
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
    [Reactive] public ushort LocalPort { get; private set; }

    public NetEngine Server { get; private set; } = null!;
    public NetEngine Client { get; private set; } = null!;
    public Session? ClientSession { get; private set; }
    public Session? ServerSession { get; private set; }

    private IServiceProvider _serviceProvider;
    private IEnumerable<SRNetwork.MessageHandler> Handlers => _serviceProvider.GetServices<SRNetwork.MessageHandler>();
    private IEnumerable<MessageHook> Hooks => _serviceProvider.GetServices<MessageHook>();
    private AgentLogin AgentLogin => _serviceProvider.GetRequiredService<AgentLogin>();

    internal void Initialize(IServiceProvider serviceProvider, IScheduler scheduler)
    {
        Server = new NetEngine(scheduler);
        Client = new NetEngine(scheduler);
        Server.Connected += Proxy_OnServerConnected;
        Client.ClientConnected += Proxy_OnClientConnected;
        
        _serviceProvider = serviceProvider;

        foreach (var packetHandler in Handlers)
        {
            Server.SetMsgHandler(packetHandler.Opcode, packetHandler.Handler);
            Client.SetMsgHandler(packetHandler.Opcode, packetHandler.Handler);
        }

        foreach (var packetHandler in Hooks)
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
        Log.Debug($"Sending to server: {packet}");

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

        await InitializeServerConnection(gatewayEndPoint);
    }

    public async Task ConnectToAgent(EndPoint agentEndPoint)
    {
        Log.Information($"Connecting to agent server at {agentEndPoint}");

        await InitializeServerConnection(agentEndPoint, true);
    }

    private async Task InitializeServerConnection(EndPoint serverEndPoint, bool agent = false)
    {
        if (ServerSession != null)
            await ServerSession.DisconnectAsync();

        if (agent)
        {
            Client.Identity = NetIdentity.AgentServer;

            Context |= ProxyContext.Agent;
            Context &= ~ProxyContext.Gateway;
        }
        else
        {
            Client.Identity = NetIdentity.GatewayServer;

            Context &= ~ProxyContext.Agent;
            Context |= ProxyContext.Gateway;
        }

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

    private async void ServerSessionOnMessageReceived(Packet packet)
    {
        Log.Debug($"Received from server: {packet}");

        if (packet.Opcode == GatewayMsgId.LoginAck)
        {
            var messageResult = (MessageResult)packet.ReadByte();
            if (messageResult != MessageResult.Success)
                return;

            packet.Reset();
            await ConnectToAgent(NetHelper.ToIPEndPoint(AgentLogin.AgentServerIp, AgentLogin.AgentServerPort));
        }

        if (packet.Opcode == AgentMsgId.LoginActionAck)
        {
            var messageResult = (MessageResult)packet.ReadByte();
            if (messageResult != MessageResult.Success)
                return;

            packet.Reset();

            _ = Task.Run(KeepAliveAgentSession);
        }

        if ((Context & ProxyContext.Client) != 0)
        {
            ClientSession?.Send(packet);
        }
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
        // if ((Context & ProxyContext.Agent) != 0)
        //     return;
        if (packet.Opcode is 0x5000 or 0x9000 or 0x2001 or 0x6103)
            return;

        ServerSession?.Send(packet);
    }

    public async Task ShutdownAsync()
    {
        await Client.StopAsync();
        await Server.StopAsync();

        Context = ProxyContext.None;
    }

    private async void KeepAliveAgentSession()
    {
        while ((Context & ProxyContext.Agent) != 0 && (Context & ProxyContext.Client) == 0)
        {
            var pingPacket = new Packet(0x2002);
            SendToServer(pingPacket);

            await Task.Delay(2000);
        }
    }

    public SRNetwork.MessageHandler? GetHandler(ushort opcode)
    {
        return Handlers.FirstOrDefault(handler => handler.Opcode == opcode);
    }

    public TMessageHandler? GetHandler<TMessageHandler>() where TMessageHandler : SRNetwork.MessageHandler
    {
        return (TMessageHandler?)Handlers.FirstOrDefault(handler => handler is TMessageHandler);
    }

    public MessageHook? GetHook(ushort opcode)
    {
        return Hooks.FirstOrDefault(hook => hook.Opcode == opcode);
    }

    public TMessageHook? GetHook<TMessageHook>() where TMessageHook : MessageHook
    {
        return (TMessageHook?)Hooks.FirstOrDefault(hook => hook is TMessageHook);
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