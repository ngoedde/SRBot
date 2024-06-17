using ReactiveUI.Fody.Helpers;
using SRGame;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityItem(RefObjItem item) : Entity(item)
{
    [Reactive] public RefObjItem RefObjItem { get; internal set; } = item;
    [Reactive] public byte OptLevel { get;internal set; }
    [Reactive] public uint Quantity { get; internal set; }
    [Reactive] public string? OwnerName { get; internal set; }
    [Reactive] public bool HasOwner { get; internal set; }
    [Reactive] public uint OwnerUniqueId { get; internal set; }
    [Reactive] public Rarity Rarity { get; internal set; }
    
    public void ParseItem(Packet packet)
    {
        if (item.TypeID2 == 1)
        {
            //ITEM_EQUIP      
            OptLevel = packet.ReadByte();
        }
        else if (item.TypeID2 == 3)
        {
            //ITEM_ETC_MONEY_GOLD
            if (item.TypeID3 == 5 && item.TypeID4 == 0)
                Quantity = packet.ReadUInt();
            
            //ITEM_ETC_TRADE
            //ITEM_ETC_QUEST
            else if (item.TypeID3 == 8 || item.TypeID3 == 9)
                OwnerName = packet.ReadString();
        }
        
        ParseEntity(packet);
        
        HasOwner = packet.ReadBool();
        if (HasOwner)
            OwnerUniqueId = packet.ReadUInt();
        Rarity = (Rarity)packet.ReadByte();
    }
}