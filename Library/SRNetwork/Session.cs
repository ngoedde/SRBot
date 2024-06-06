using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using SRNetwork.Common;
using SRNetwork.Common.Profiling;
using SRNetwork.SilkroadSecurityApi;

namespace SRNetwork;

public class Session
{
    public delegate void MessageReceivedEventHandler(Packet packet);
    public delegate void MessageSentEventHandler(Packet packet);
    
    public event MessageReceivedEventHandler? MessageReceived;
    public event MessageSentEventHandler? MessageSent;
    
    public delegate void DisconnectedEventHandler(DisconnectReason reason);
    public event DisconnectedEventHandler? Disconnected;
    
    private readonly Socket _socket;
    private readonly Security _security;
    private readonly PacketHandlerManager _handlerManager;
    
    private bool _disconnect;
    private Task _task = null!;

    public int Id { get; set; }

    public NetTrafficProfile? Profiler { get; set; }
    
    public EndPoint? RemoteEndPoint => _socket.RemoteEndPoint;

    public Session(int id, Socket socket, Security security, PacketHandlerManager handlerManager)
    {
        this.Id = id;

        _socket = socket;
        _security = security;
        _handlerManager = handlerManager;
    }

    public void Start(CancellationToken cancellationToken = default)
    {
        _task = this.ReceiveAsync(cancellationToken);
    }

    public async ValueTask<bool> Send(Packet packet)
    {
        if (_disconnect)
            return false;

        _security.Send(packet);
        await this.TransferOutgoing().ConfigureAwait(false);
        
        OnMessageSent(packet);
        
        return true;
    }

    public async Task DisconnectAsync()
    {
        await _socket.DisconnectAsync(false);
        
        OnDisconnected(DisconnectReason.EngineError);
    }

    private async ValueTask Transfer()
    {
        await this.TransferIncoming().ConfigureAwait(false);
        await this.TransferOutgoing().ConfigureAwait(false);
    }
    
    private async ValueTask TransferOutgoing()
    {
        var packets = _security.TransferOutgoing();
        if (packets != null)
        {
            foreach (var kvp in packets)
            {
                //Console.WriteLine($"Session#{this.Id} sent {kvp.Value.Opcode:X4}");
                var buffer = kvp.Key;
                await _socket.SendAsync(buffer.AsMemory()).ConfigureAwait(false);
                this.Profiler?.ReportSent(buffer.Size);
            }
        }
    }

    private async ValueTask TransferIncoming()
    {
        var packets = _security.TransferIncoming();
        if (packets != null)
        {
            foreach (var packet in packets)
            {
                //Console.WriteLine($"Session#{this.Id} received {packet.Opcode:X4}");

                if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000)
                    continue;
                
                await _handlerManager.Handle(this, packet);
            }
        }
    }

    private async Task ReceiveAsync(CancellationToken cancellationToken = default)
    {
        var pinnedArray = GC.AllocateUninitializedArray<byte>(8192, pinned: true);
        var pinnedMemory = MemoryMarshal.CreateFromPinnedArray(pinnedArray, 0, pinnedArray.Length);
        try
        {
            // Sends out Handshake if present.
            await this.Transfer().ConfigureAwait(false);

            while (!cancellationToken.IsCancellationRequested)
            {
                var bytesReceived = await _socket.ReceiveAsync(pinnedMemory, SocketFlags.None, cancellationToken).ConfigureAwait(false);
                if (bytesReceived == 0)
                {
                    await this.DisconnectAsync(DisconnectReason.ClosedByPeer, cancellationToken).ConfigureAwait(false);
                    break;
                }

                this.Profiler?.ReportReceive(bytesReceived);
                _security.Recv(pinnedArray, 0, bytesReceived);
                await this.Transfer().ConfigureAwait(false);
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode is SocketError.ConnectionReset)
        {
            await this.DisconnectAsync(DisconnectReason.ClosedByPeer, cancellationToken).ConfigureAwait(false);
        }
        catch (SocketException ex) when (ex.SocketErrorCode is SocketError.TimedOut)
        {
            await this.DisconnectAsync(DisconnectReason.TimeOut, cancellationToken).ConfigureAwait(false);
        }
        catch (SocketException ex)
        {
            Console.WriteLine(ex.Message);
            await this.DisconnectAsync(DisconnectReason.ReceiveError, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await this.DisconnectAsync(DisconnectReason.EngineError, cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask<bool> DisconnectAsync(DisconnectReason reason, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Disconnected: {reason}");
        _disconnect = true;
        await _socket.DisconnectAsync(false, cancellationToken);
        //using var msg = _allocator.NewLocalMsg(NetMsgID.LOCAL_NET_DISCONNECT);
        //return _poster.PostMsg(msg);
        return true;
    }

    protected virtual void OnDisconnected(DisconnectReason reason)
    {
        Disconnected?.Invoke(reason);
    }

    protected virtual void OnMessageReceived(Packet packet)
    {
        MessageReceived?.Invoke(packet);
    }

    protected virtual void OnMessageSent(Packet packet)
    {
        MessageSent?.Invoke(packet);
    }
}
