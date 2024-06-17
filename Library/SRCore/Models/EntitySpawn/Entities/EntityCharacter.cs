using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.Inventory;
using SRGame;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityCharacter(RefObjChar refObjChar) : EntityBionic(refObjChar)
{
    [Reactive] public byte Scale { get; internal set; }
    [Reactive] public byte HwanLevel { get; internal set; }
    [Reactive] public PvpCape PvpCape { get; internal set; }
    [Reactive] public AutoInvestFlag AutoInvestFlag { get; internal set; }
    [Reactive] public ObservableCollection<InventoryItemBasic> Inventory { get; internal set; } = new();
    [Reactive] public ObservableCollection<InventoryItemBasic> AvatarInventory { get; internal set; } = new();
    [Reactive] public bool HasMask { get; internal set; }

    public void ParseCharacter(RefObjChar refObjChar, EntityManager entityManager, Packet packet)
    {
        Scale = packet.ReadByte();
        HwanLevel = packet.ReadByte();
        PvpCape = (PvpCape)packet.ReadByte();
        AutoInvestFlag = (AutoInvestFlag)packet.ReadByte();

        var inventorySize = packet.ReadByte();
        var inventoryCount = packet.ReadByte();
        for (var i = 0; i < inventoryCount; i++)
        {
            var refObjItemId = packet.ReadInt();
            var refObjItem = entityManager.GetItem(refObjItemId);
            
            if (refObjItem == null)
                throw new Exception("EntityCharacter: refObjItem is null");
            
            if (refObjItem.TypeID1 == 3 && refObjItem.TypeID2 == 1)
                Inventory.Add(new InventoryItemBasic(refObjItem)
                {
                    OptLevel = packet.ReadByte()
                });
        }
        
        var avatarInventorySize = packet.ReadByte();
        var avatarInventoryCount = packet.ReadByte();
        for (var i = 0; i < avatarInventoryCount; i++)
        {
            var refObjItemId = packet.ReadInt();
            var refObjItem = entityManager.GetItem(refObjItemId);
            
            if (refObjItem == null)
                throw new Exception("EntityCharacter: refObjItem is null");
            
            if (refObjItem.TypeID1 == 3 && refObjItem.TypeID2 == 1)
                AvatarInventory.Add(new InventoryItemBasic(refObjItem)
                {
                    OptLevel = packet.ReadByte()
                });
        }
        
        HasMask = packet.ReadBool();
        if (HasMask)
        {
            var maskRefObjItemId = packet.ReadInt();
            var maskRefObjItem = entityManager.GetItem(maskRefObjItemId);
            
            if (maskRefObjItem == null)
                throw new Exception("EntityCharacter: maskRefObjItem is null");

            if (maskRefObjItem.TypeID1 == refObjChar.TypeID1 && maskRefObjItem.TypeID2 == refObjChar.TypeID2)
            {
                packet.ReadByte(); //Mask.Scale
                var duplicateItemCount = packet.ReadByte();
                for (var i = 0; i < duplicateItemCount; i++)
                {
                    packet.ReadInt();//Mask.Items.RefItemId
                }
            }
        }
    }
}