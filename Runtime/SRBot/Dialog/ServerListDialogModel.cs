using ReactiveUI;
using SRCore.Models;

namespace SRBot.Dialog;

public class ServerListDialogModel : ReactiveObject
{
    public ServerListDialogModel(ShardList shardList)
    {
        ShardList = shardList;
    }
    
    public ShardList ShardList { get; } 
}