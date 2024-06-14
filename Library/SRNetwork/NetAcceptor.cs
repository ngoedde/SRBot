using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using SRNetwork.Common;
using SRNetwork.Common.Extensions;

namespace SRNetwork;

public class NetAcceptor
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly Socket _listener;
    private Task _task = null!;

    public delegate void NetAcceptedEventHandler(Socket socket);

    private readonly NetAcceptedEventHandler _accepted;

    public NetAcceptor(NetAcceptedEventHandler accepted)
    {
        _accepted = accepted;

        _listener = NetHelper.CreateTcpSocket();
        _listener.Optimize();
    }

    public void Start(ushort port)
    {
        _listener.Bind(new IPEndPoint(IPAddress.Any, port));
        _listener.Listen(128);

        _task = this.AcceptAsync();
    }

    public async Task StopAsync()
    {
        _cts.Cancel();
        _listener.Close();
        if (_task == null)
            return;

        await _task.ConfigureAwait(false);
    }

    private async Task AcceptAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            //var socket = _socketPool.Rent();
            var socket = NetHelper.CreateTcpSocket();

            var acceptedSocket = await _listener.AcceptAsync(socket, cancellationToken);
            Debug.Assert(acceptedSocket == socket);

            Console.WriteLine($"Accepted connection from {acceptedSocket.RemoteEndPoint}");

            _accepted(acceptedSocket);
        }
    }
}