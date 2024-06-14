using System.Net;
using System.Net.Sockets;
using System.Text;
using SRNetwork.Common;
using SRNetwork.Common.Profiling;
using SRNetwork.SilkroadSecurityApi;
using System.Reactive.Concurrency;
namespace SRNetwork;

public class NetEngine
{
    private readonly IScheduler _scheduler;

    public delegate void ClientConnectedEventHandler(Session session);

    public delegate void ClientDisconnectedEventHandler(Session session);

    public delegate void ConnectedEventHandler(Session session);

    public delegate void DisconnectedEventHandler(Session session);

    public event ClientConnectedEventHandler? ClientConnected;
    public event ClientDisconnectedEventHandler? ClientDisconnected;

    public event ConnectedEventHandler? Connected;
    public event DisconnectedEventHandler? Disconnected;

    private readonly IDGenerator32 _generator;
    private readonly NetAcceptor _acceptor;
    private readonly NetConnector _connector;
    private readonly SessionManager _sessionManager;
    private readonly PacketHandlerManager _packetHandlerManager;

    public IReadOnlyCollection<Session> Sessions => _sessionManager;

    public NetTrafficProfiler Profiler { get; }

    public string Identity { get; set; } = NetIdentity.GatewayServer;

    public NetEngine(IScheduler scheduler)
    {
        _scheduler = scheduler;
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _generator = new IDGenerator32();
        _acceptor = new NetAcceptor(this.OnAccepted);
        _connector = new NetConnector(this.OnConnected);
        _packetHandlerManager = new PacketHandlerManager(scheduler);
        _sessionManager = new SessionManager(_generator, _packetHandlerManager);

        this.Profiler = new NetTrafficProfiler();
    }


    public async Task StopAsync()
    {
        await _acceptor.StopAsync();

        foreach (var session in _sessionManager)
        {
            // if (session.)
            await session.DisconnectAsync();
        }

        _sessionManager.Clear();
    }

    private void OnAccepted(Socket socket)
    {
        var security = new Security();
        security.GenerateSecurity(true, true, true);
        security.ChangeIdentity(Identity, 0);

        var session = _sessionManager.CreateSession(_scheduler, socket, security);
        session.Profiler = this.Profiler.AddProfile(session.Id);
        session.Start();

        OnClientConnected(session);
    }

    private void OnConnected(Socket socket)
    {
        var security = new Security();
        var session = _sessionManager.CreateSession(_scheduler, socket, security);
        session.Profiler = this.Profiler.AddProfile(session.Id);
        session.Start();

        OnConnected(session);
    }

    public void SetMsgHandler(ushort id, PacketHandler handler)
    {
        _packetHandlerManager.SetMsgHandler(id, handler);
    }

    public void SetMsgHook(ushort id, PacketHook hook)
    {
        _packetHandlerManager.SetMsgHook(id, hook);
    }

    public void Start(ushort port)
        => _acceptor.Start(port);

    public ValueTask ConnectAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
    {
        return _connector.ConnectAsync(endPoint, cancellationToken);
    }

    protected virtual void OnClientConnected(Session session)
    {
        ClientConnected?.Invoke(session);
    }

    protected virtual void OnClientDisconnected(Session session)
    {
        ClientDisconnected?.Invoke(session);
    }

    protected virtual void OnConnected(Session session)
    {
        Connected?.Invoke(session);
    }

    protected virtual void OnDisconnected(Session session)
    {
        Disconnected?.Invoke(session);
    }
}