using ReactiveUI;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public abstract class GameModel(IServiceProvider serviceProvider) : ReactiveObject
{
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;

    internal virtual bool TryParsePacket(Session session, Packet packet)
    {
        return true;
    }
}