using System.Net;
using System.Net.Sockets;
using SRNetwork.Common;
using SRNetwork.Common.Extensions;

namespace SRNetwork;

public class NetConnector
{
    public delegate void NetConnectedEventHandler(Socket socket);

    private readonly NetConnectedEventHandler _connected;

    public NetConnector(NetConnectedEventHandler connected)
    {
        _connected = connected;
    }

    public ValueTask ConnectAsync(string hostOrIP, ushort port, CancellationToken cancellationToken = default)
        => this.ConnectAsync(NetHelper.ToIPEndPoint(hostOrIP, port), cancellationToken);

    public async ValueTask ConnectAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
    {
        // TODO: SocketPool or SessionPool

        var socket = NetHelper.CreateTcpSocket();
        socket.Optimize();

        try
        {
            await socket.ConnectAsync(endPoint, cancellationToken).ConfigureAwait(false);

            _connected(socket);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}