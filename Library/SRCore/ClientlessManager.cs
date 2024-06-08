using Serilog;
using SRCore.Config;
using SRCore.Config.Model;
using SRCore.Models;
using SRNetwork;

namespace SRCore;

/// <summary>
/// Automatically handles clientless/client mode.
/// </summary>
public class ClientlessManager
{
    private readonly PatchInfo _patchInfo;
    private readonly ShardList _shardList;
    private readonly ProfileService _profileService;
    private readonly Proxy _proxy;
    private Profile ActiveProfile => _profileService.ActiveProfile;
    
    public ClientlessManager(PatchInfo patchInfo, ShardList shardList, ProfileService profileService, Proxy proxy)
    {
        _patchInfo = patchInfo;
        _shardList = shardList;
        _profileService = profileService;
        _proxy = proxy;
        
        _patchInfo.PatchInfoUpdated += OnPatchInfoUpdated;
        _proxy.GatewayConnected += ProxyOnGatewayConnected;
    }

    private void ProxyOnGatewayConnected(Session serverSession)
    {
        if (ActiveProfile.Clientless)
            _patchInfo.Request();
    }
    
    private async void OnPatchInfoUpdated(Session session, PatchInfo patchInfo)
    {
        if (patchInfo.PatchRequired)
        {
            Log.Fatal("!!! Game update available !!! Please run Silkroad.exe to update the client.");
            
            await session.DisconnectAsync();
            
            return;
        }

        if (ActiveProfile.Clientless)
            _shardList.Request();
    }
}