using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public delegate ValueTask<bool> PacketHandler(Session session, Packet packet);

public class PacketHandlerManager
{
    private readonly IDictionary<ushort, PacketHandler> _handlerMap;

    public PacketHandlerManager()
    {
        _handlerMap = new Dictionary<ushort, PacketHandler>();
    }

    public PacketHandler this[ushort id]
    {
        get => _handlerMap[id];
        set => _handlerMap[id] = value;
    }

    public void SetMsgHandler(ushort id, PacketHandler handler) => _handlerMap[id] = handler;

    public ValueTask<bool> Handle(Session session, Packet packet)
    {
        if (!_handlerMap.TryGetValue(packet.Opcode, out var handler))
            return ValueTask.FromResult(false);

        return handler(session, packet);
    }
}