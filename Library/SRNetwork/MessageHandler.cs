using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public abstract class MessageHandler
{
    public virtual PacketHandler Handler { get; init; }
    public virtual ushort Opcode { get; init; }

    public virtual ValueTask<bool> Handle(Session session, Packet packet)
    {
        return Handler(session, packet);
    }
}