using ReactiveUI;
using SRCore.Models;
using SukiUI.Controls;

namespace SRBot.Dialog;

public class ServerListDialogModel(ShardList shardList) : ReactiveObject
{
    public ShardList ShardList => shardList;

    public void RefreshServerList()
    {
        ShardList.Request();
    }

    public void CloseDialog()
    {
        SukiHost.CloseDialog();
    }
}