using System.Net.Sockets;
using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public class ClientSession : Session
{
    public ClientSession(int id, Socket socket, Security security, PacketHandlerManager handlerManager) : base(id, socket, security, handlerManager)
    {
        
    }
}