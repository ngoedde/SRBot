using System.Collections.ObjectModel;
using SRCore.Models.Inventory.ItemType;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.Inventory;

internal class ItemFactory
{
    public static Item ParseFromPacket(Packet packet, EntityManager entityManager)
    {
        var slot = packet.ReadByte();
        var itemRentInfo = ParseRentInfoFromPacket(packet);
        var itemId = packet.ReadInt();

        var item = entityManager.GetItem(itemId);
        if (item == null)
            throw new Exception($"Item with id {itemId} not found in entity manager!");

        //Parse gear
        if (item.TypeID2 == 1)
            return ParseItemEquip(packet, item, slot, itemRentInfo);

        if (item.TypeID2 == 2)
        {
            if (item.TypeID3 == 1)
                return ParseItemCosSummoner(packet, item, slot, itemRentInfo);

            if (item.TypeID3 == 2)
                return ParseItemMonsterCapsule(packet, item, slot, itemRentInfo);

            if (item.TypeID3 == 3)
                return ParseItemStorage(packet, item, slot, itemRentInfo);
        }

        if (item.TypeID2 == 3)
            return ParseItemExpendable(packet, item, slot, itemRentInfo);

        throw new Exception("Can not parse inventory! Unknown item type!");
    }

    private static ItemEquip ParseItemEquip(Packet packet, RefObjItem item, byte slot, ItemRentInfo? rentInfo)
    {
        var result = new ItemEquip(slot, item, rentInfo);

        result.OptLevel = packet.ReadByte();
        result.Variance = packet.ReadULong();
        result.Durability = packet.ReadUInt();
        result.MagicParams = ParseMagicParams(packet);
        result.BindingOptionTypeA = packet.ReadByte();
        result.BindingOptionsA = ParseBindingOptions(packet);
        result.BindingOptionTypeB = packet.ReadByte();
        result.BindingOptionsB = ParseBindingOptions(packet);

        return result;
    }

    private static ItemCosSummoner ParseItemCosSummoner(Packet packet, RefObjItem item, byte slot,
        ItemRentInfo? rentInfo)
    {
        var result = new ItemCosSummoner(slot, item, rentInfo);

        result.State = (SummonerState)packet.ReadByte();
        if (result.State != SummonerState.Inactive)
        {
            result.ContainedRefObjId = packet.ReadUInt();
            result.Name = packet.ReadString();

            //ITEM_COS_P (Ability pets)
            if (item.TypeID4 == 2)
            {
                result.SecondsToRentEndTime = packet.ReadUInt();
            }

            var timedJobCount = packet.ReadByte();
            for (var i = 0; i < timedJobCount; i++)
            {
                var timedJob = new TimedJob
                {
                    Category = packet.ReadByte(),
                    JobId = packet.ReadUInt(),
                    TimeToKeep = packet.ReadUInt()
                };

                if (timedJob.Category == 5)
                {
                    packet.ReadUInt(); //Unknown data
                    packet.ReadByte(); //Unknown data
                }

                result.TimedJobs.Add(timedJob);
            }
        }

        return result;
    }

    private static ItemExpendable ParseItemExpendable(Packet packet, RefObjItem item, byte slot, ItemRentInfo? rentInfo)
    {
        var result = new ItemExpendable(slot, item, rentInfo)
        {
            Quantity = packet.ReadUShort()
        };

        if (item.TypeID3 == 11)
        {
            //MAGICSTONE, ATTRSTONE
            if (item.TypeID4 == 1 || item.TypeID4 == 2)
                result.AttributeAssimilationProbability = packet.ReadByte(); //AttributeAssimilationProbability
        }
        else if (item.TypeID3 == 14 && item.TypeID4 == 2)
        {
            //ITEM_MALL_GACHA_CARD_WIN
            //ITEM_MALL_GACHA_CARD_LOSE
            var magicParamCount = packet.ReadByte();
            for (var i = 0; i < magicParamCount; i++)
            {
                var key = packet.ReadUInt();
                var value = packet.ReadUInt();

                result.MagicParams[key] = value;
            }
        }

        return result;
    }

    private static ItemMonsterCapsule ParseItemMonsterCapsule(Packet packet, RefObjItem item, byte slot,
        ItemRentInfo? rentInfo)
    {
        var result = new ItemMonsterCapsule(slot, item, rentInfo)
        {
            ContainedRefObjId = packet.ReadUInt()
        };

        return result;
    }

    private static ItemStorage ParseItemStorage(Packet packet, RefObjItem item, byte slot, ItemRentInfo? rentInfo)
    {
        var result = new ItemStorage(slot, item, rentInfo)
        {
            Quantity = packet.ReadUInt()
        };

        return result;
    }


    private static ObservableCollection<BindingOption> ParseBindingOptions(Packet packet)
    {
        var count = packet.ReadByte();
        var bindingOptions = new ObservableCollection<BindingOption>();

        for (var i = 0; i < count; i++)
        {
            var option = new BindingOption
            {
                Slot = packet.ReadByte(),
                Id = packet.ReadUInt(),
                Value = packet.ReadUInt()
            };

            bindingOptions.Add(option);
        }

        return bindingOptions;
    }

    private static Dictionary<uint, uint> ParseMagicParams(Packet packet)
    {
        var count = packet.ReadByte();
        var magicParams = new Dictionary<uint, uint>();

        for (var i = 0; i < count; i++)
        {
            var key = packet.ReadUInt();
            var value = packet.ReadUInt();

            magicParams[key] = value;
        }

        return magicParams;
    }

    private static ItemRentInfo? ParseRentInfoFromPacket(Packet packet)
    {
        var rentType = packet.ReadUInt();

        if (rentType == 1)
        {
            return new ItemRentInfo
            {
                CanDelete = packet.ReadUShort(),
                PeriodBeginTime = packet.ReadUInt(),
                PeriodEndTime = packet.ReadUInt(),
            };
        }

        if (rentType == 2)
        {
            return new ItemRentInfo
            {
                CanDelete = packet.ReadUShort(),
                CanRecharge = packet.ReadUShort(),
                MeterRateTime = packet.ReadUInt()
            };
        }

        if (rentType == 3)
        {
            return new ItemRentInfo
            {
                CanDelete = packet.ReadUShort(),
                CanRecharge = packet.ReadUShort(),
                PeriodBeginTime = packet.ReadUInt(),
                PeriodEndTime = packet.ReadUInt(),
                PackingTime = packet.ReadUInt()
            };
        }

        return null;
    }
}