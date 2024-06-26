﻿using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reactive.Concurrency;
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

    public Session CreateSession(IScheduler scheduler, Socket socket, Security security)
    {
        var id = _generator.Next();
        var session = new Session(id, socket, security, _packetHandlerManager);
        return _sessions[id] = session;
    }

    public bool TryFindSessionById(int id, [MaybeNullWhen(false)] out Session session) =>
        _sessions.TryGetValue(id, out session);

    public int Count => _sessions.Count;

    public IEnumerator<Session> GetEnumerator() => _sessions.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _sessions.Values.GetEnumerator();

    public void Clear()
    {
        _sessions.Clear();
    }
}