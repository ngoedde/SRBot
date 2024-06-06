using Serilog;
using SRCore.Models;
using SRNetwork;

namespace SRCore;

public class ClientlessManager
{
    private readonly PatchInfo _patchInfo;
    private readonly ShardList _shardList;

    public ClientlessManager(PatchInfo patchInfo, ShardList shardList)
    {
        _patchInfo = patchInfo;
        _shardList = shardList;

        _patchInfo.PatchInfoUpdated += OnPatchInfoUpdated;
    }

    public void RequestPatchInfoAfterGatewayConnect()
    {
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

        Log.Information("The client is up to date. Requesting shard list.");
        _shardList.Request();
    }
}