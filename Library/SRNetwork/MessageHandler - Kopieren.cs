using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public abstract class MessageHook
{
    public virtual PacketHook Hook { get; init; }
    public virtual ushort Opcode { get; init; }

    public virtual ValueTask<Packet> Handle(Session session, Packet packet)
    {
        return Hook(session, packet);
    }
}