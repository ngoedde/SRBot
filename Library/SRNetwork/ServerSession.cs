using System.Net.Sockets;
using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public class ServerSession : Session
{
    public ServerSession(int id, Socket socket, Security security, PacketHandlerManager handlerManager) : base(id, socket, security, handlerManager)
    {
    }
}