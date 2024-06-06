using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using SRNetwork.Common;
using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

internal class SessionManager : IReadOnlyCollection<Session>
{
    private readonly IDGenerator32 _generator;
    private readonly ConcurrentDictionary<int, Session> _sessions = new ConcurrentDictionary<int, Session>();
    private readonly PacketHandlerManager _packetHandlerManager;

    public SessionManager(IDGenerator32 generator, PacketHandlerManager packetHandlerManager)
    {
        _packetHandlerManager = packetHandlerManager;
        _generator = generator;
    }

    public Session CreateSession(Socket socket, Security security)
    {
        var id = _generator.Next();
        var session = new Session(id, socket, security, _packetHandlerManager);
        return _sessions[id] = session;
    }

    public TSession CreateSession<TSession>(Socket socket, Security security) where TSession : Session
    {
        var id = _generator.Next();
        var session = (TSession)Activator.CreateInstance(typeof(TSession), id, socket, security, _packetHandlerManager);
    
        if (session == null)
        {
            throw new InvalidOperationException($"Could not create an instance of type {typeof(TSession).FullName}.");
        }

        _sessions[id] = session;
        return session;
    }
    
    public bool TryFindSessionById(int id, [MaybeNullWhen(false)] out Session session) => _sessions.TryGetValue(id, out session);

    public int Count => _sessions.Count;

    public IEnumerator<Session> GetEnumerator() => _sessions.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _sessions.Values.GetEnumerator();

    public void Clear()
    {
        _sessions.Clear();
    }
}