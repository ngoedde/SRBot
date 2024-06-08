using System.Collections.Generic;
using ReactiveUI;
using SRCore.Models;

namespace SRBot.Dialog;

public class ServerListDialogModel(ShardList shardList) : ReactiveObject
{
    public ShardList ShardList => shardList;

    public void RefreshServerList()
    {
        ShardList.Request();

        this.RaisePropertyChanged(nameof(ShardList.Shards));
    }
}