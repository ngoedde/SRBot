using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public abstract class GameModel(IServiceProvider serviceProvider) : ReactiveObject
{
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;
    protected Proxy Proxy => ServiceProvider.GetRequiredService<Proxy>();

    internal virtual bool TryParsePacket(Session session, Packet packet)
    {
        return true;
    }
}