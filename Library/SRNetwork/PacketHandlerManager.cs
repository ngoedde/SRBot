using System.Reactive.Concurrency;
using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public delegate ValueTask<bool> PacketHandler(Session session, Packet packet);

public delegate ValueTask<Packet> PacketHook(Session session, Packet packet);

public class PacketHandlerManager(IScheduler scheduler)
{
    private readonly IDictionary<ushort, PacketHandler> _handlerMap = new Dictionary<ushort, PacketHandler>();
    private readonly IDictionary<ushort, PacketHook> _hookMap = new Dictionary<ushort, PacketHook>();

    public PacketHandler this[ushort id]
    {
        get => _handlerMap[id];
        set => _handlerMap[id] = value;
    }

    public void SetMsgHandler(ushort id, PacketHandler handler) => _handlerMap[id] = handler;
    public void SetMsgHook(ushort id, PacketHook hook) => _hookMap[id] = hook;

    public ValueTask<bool> Handle(Session session, Packet packet)
    {
        if (!_handlerMap.TryGetValue(packet.Opcode, out var handler))
            return ValueTask.FromResult(false);

   
        scheduler.Schedule(session, (scheduler1, session1) =>
        {
            if (handler != null) handler(session, packet);
        });
        
        return ValueTask.FromResult<bool>(true);
    }

    public ValueTask<Packet> Hook(Session session, Packet packet)
    {
        if (!_hookMap.TryGetValue(packet.Opcode, out var hook))
            return ValueTask.FromResult(packet);

        if (!packet.Locked)
            packet.Lock();

        return hook(session, packet);
    }
}