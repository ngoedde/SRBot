using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.ShardInfo;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class ShardList(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    public delegate void ShardListUpdatedHandler(ShardList shardList);
    public event ShardListUpdatedHandler? ShardListUpdated;
    
    [Reactive] public ObservableCollection<Shard> Shards { get; internal set; } = new();
    [Reactive] public ObservableCollection<Farm> Farms { get; internal set; } = new();
    
    private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();

    internal override bool TryParsePacket(Session session, Packet packet)
    {
        Shards = new ObservableCollection<Shard>();
        Farms = new ObservableCollection<Farm>();

        try
        {
            var hasNextFarmEntry = packet.ReadBool();
            while (hasNextFarmEntry)
            {
                var farmId = packet.ReadByte();
                var farmName = packet.ReadString();

                Farms.Add(new Farm
                {
                    Id = farmId,
                    Name = farmName
                });

                hasNextFarmEntry = packet.ReadBool();
            }

            var hasNextShardEntry = packet.ReadBool();
            while (hasNextShardEntry)
            {
                var shardId = packet.ReadUShort();
                var shardName = packet.ReadString();
                var shardOnlineCount = packet.ReadUShort();
                var shardCapacity = packet.ReadUShort();
                var shardIsOperating = packet.ReadBool();
                var shardFarmId = packet.ReadByte();

                Shards.Add(new Shard
                {
                    Id = shardId,
                    Name = shardName,
                    Capacity = shardCapacity,
                    OnlineCount = shardOnlineCount,
                    Operating = shardIsOperating,
                    FarmId = shardFarmId
                });

                hasNextShardEntry = packet.ReadBool();
            }
        }
        catch (Exception)
        {
            return false;
        }

        OnShardListUpdated(this);

        return true;
    }
    
    public void Request()
    {
        var packet = new Packet(GatewayMsgId.ShardInfoReq);
        
        _proxy.SendToServer(packet);
    }

    protected virtual void OnShardListUpdated(ShardList shardList)
    {
        ShardListUpdated?.Invoke(shardList);
    }
}