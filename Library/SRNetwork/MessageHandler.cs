using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public abstract class MessageHandler
{
    public delegate void HandledEventHandler(MessageHandler handler, Session session, Packet packet);

    public event HandledEventHandler? Handled;

    public virtual PacketHandler Handler { get; init; } = null!;
    public virtual ushort Opcode { get; init; }

    public abstract ValueTask<bool> Handle(Session session, Packet packet);

    protected ValueTask<bool> OnHandled(Session session, Packet packet, Exception? exception = null)
    {
#if DEBUG
        if (exception != null)
            throw exception;
#endif

        packet.Reset();

        Handled?.Invoke(this, session, packet);

        return ValueTask.FromResult(exception == null);
    }
}