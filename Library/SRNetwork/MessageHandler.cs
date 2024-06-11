using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public abstract class MessageHandler
{
    public delegate void HandledEventHandler(Session session, Packet packet);
    public event HandledEventHandler? Handled;
    
    public virtual PacketHandler Handler { get; init; } = null!;
    public virtual ushort Opcode { get; init; }

    public abstract ValueTask<bool> Handle(Session session, Packet packet);

    protected ValueTask<bool> OnHandled(Session session, Packet packet)
    {
        packet.Reset();
        
        Handled?.Invoke(session, packet);
        
        return ValueTask.FromResult(true);
    }
}