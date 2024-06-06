using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.Patch;
using SRGame.Client;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class PatchInfo(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    public delegate void PatchInfoUpdatedHandler(Session session, PatchInfo patchInfo);
    public event PatchInfoUpdatedHandler? PatchInfoUpdated;

    private readonly ClientInfoManager _clientInfoManager = serviceProvider.GetRequiredService<ClientInfoManager>();
    
    [Reactive] public PatchErrorCode ErrorCode { get; internal set; }
    public bool PatchRequired => ErrorCode > 0;
    [Reactive] public uint LatestVersion { get; internal set; }
    [Reactive] public string DownloadServerIp { get; internal set; }
    [Reactive] public ushort DownloadServerPort { get; internal set; }

    private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();

    public void Request()
    {
        var packet = new Packet(0x6100);

        packet.WriteByte(_clientInfoManager.DivisionInfo.ContentId);
        packet.WriteString(NetIdentity.SilkroadClient);
        packet.WriteUInt32(_clientInfoManager.Version);

        _proxy.SendToServer(packet);
    }

    internal override bool TryParsePacket(Session session, Packet packet)
    {
        var messageResult = (MessageResult)packet.ReadByte();
        if (messageResult == MessageResult.Error)
        {
            ErrorCode = (PatchErrorCode) packet.ReadByte();
            if (ErrorCode == PatchErrorCode.UPDATE)
            {
                
                DownloadServerIp = packet.ReadString();
                DownloadServerPort = packet.ReadUShort();
                LatestVersion = packet.ReadUInt();
            }
        }

        OnPatchInfoUpdated(session, this);

        return true;
    }

    protected virtual void OnPatchInfoUpdated(Session session, PatchInfo patchInfo)
    {
        PatchInfoUpdated?.Invoke(session, patchInfo);
    }
}