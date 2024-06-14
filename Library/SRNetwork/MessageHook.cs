using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public abstract class MessageHook
{
    public delegate void HookedEventHandler(MessageHook hook, Session session, Packet packet);

    public event HookedEventHandler? Hooked;

    public virtual PacketHook Hook { get; init; }
    public virtual ushort Opcode { get; init; }

    public abstract ValueTask<Packet> Handle(Session session, Packet packet);

    protected ValueTask<Packet> OnHooked(Session session, Packet packet, Exception? exception = null)
    {
#if DEBUG
        if (exception != null)
            throw exception;
#endif

        packet.Reset();

        Hooked?.Invoke(this, session, packet);

        return ValueTask.FromResult(packet);
    }
}