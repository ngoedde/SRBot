using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using Serilog.Events;
using SRCore.Models.EntitySpawn;
using SRCore.Models.EntitySpawn.Entities;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Spawn(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    [Reactive] public ObservableCollection<Entity> Entities { get; internal set; } = new();

    internal Packet GroupSpawnPacket { get; set; } = new Packet(AgentMsgId.GroupSpawnData);
    internal ushort GroupSpawnCount { get; set; } = 0;
    internal GroupSpawnType GroupSpawnType { get; set; } = 0;
    private EntityManager EntityManager => serviceProvider.GetRequiredService<EntityManager>();
    private Kernel Kernel => serviceProvider.GetRequiredService<Kernel>();

    internal override void ParsePacket(Session session, Packet packet)
    {
        for (var i = 0; i < GroupSpawnCount; i++)
        {
            if (GroupSpawnType == GroupSpawnType.Despawn)
            {
                var despawnUniqueId = packet.ReadUInt();
                var entity = Entities.FirstOrDefault(x => x.UniqueId == despawnUniqueId);
                if (entity != null)
                {
                    Entities.Remove(entity);
                }

                return;
            }
            var refObjId = packet.ReadUInt();
            if (refObjId == uint.MaxValue)
            {
                ParseEventZone(packet);
            }

            var refObjCommon = EntityManager.GetRefObjCommon((int)refObjId);
            if (refObjCommon == null)
            {
                _ = Kernel.Panic($"Spawn: refObjCommon for id {refObjId} is null", LogEventLevel.Warning, false);

                return;
            }

            if (refObjCommon.TypeID1 == 1 && refObjCommon is RefObjChar objChar)
            {
                ParseBionic(objChar, packet);
                return;
            }
            else if (refObjCommon.TypeID1 == 3 && refObjCommon is RefObjItem refObjItem)
            {
                ParseItem(refObjItem, packet);

                return;
            }
            else if (refObjCommon.TypeID1 == 4)
            {
                ParsePortal(refObjCommon, packet);
            }

            throw new Exception($"Spawn: TypeID1 `{refObjCommon.TypeID1}` not implemented!");
        }
    }

    private void ParseBionic(RefObjChar refObjChar, Packet packet)
    {
        if (refObjChar == null)
        {
            _ = Kernel.Panic("Spawn: refObjChar is null", LogEventLevel.Warning, false);
            return;
        }

        //BIONIC:
        //  CHARACTER
        //  NPC
        //      NPC_FORTRESS_STRUCT
        //      NPC_MOB
        //      NPC_COS
        //      NPC_FORTRESS_COS    
        if (refObjChar.TypeID2 == 1)
        {
            var player = new EntityPlayer(refObjChar);
            player.ParseCharacter(refObjChar, EntityManager, packet);
            player.ParseEntity(packet);
            player.ParseBionic(packet, EntityManager);
            player.ParsePlayer(packet);

            Entities.Add(player);
        }
        else if (refObjChar.TypeID2 == 2 && refObjChar.TypeID3 == 5)
        {
            var structure = new EntityStructure(refObjChar);
            structure.ParseStructure(packet);
            structure.ParseEntity(packet);
            structure.ParseBionic(packet, EntityManager);

            Entities.Add(structure);
        }
        else if (refObjChar.TypeID2 == 2)
        {
            if (refObjChar.TypeID3 == 1)
            {
                var monster = new EntityMonster(refObjChar);
                monster.ParseEntity(packet);
                monster.ParseBionic(packet, EntityManager);
                monster.ParseNpc(packet);
                monster.ParseMonster(refObjChar, packet);

                Entities.Add(monster);
            }
            else if (refObjChar.TypeID3 == 3) //NPC_COS
            {
                var cos = new EntityCos(refObjChar);
                cos.ParseEntity(packet);
                cos.ParseBionic(packet, EntityManager);
                cos.ParseCos(packet);

                Entities.Add(cos);
            }
            else if (refObjChar.TypeID3 == 5)
            {
                var fortressCos = new EntityNpc(refObjChar);
                fortressCos.ParseEntity(packet);
                fortressCos.ParseBionic(packet, EntityManager);

                packet.ReadUInt(); //NPC_FORTESS_COS
                packet.ReadString(); //Guild name

                Entities.Add(fortressCos);
            }
            else
            {
                var npc = new EntityNpc(refObjChar);
                npc.ParseEntity(packet);
                npc.ParseBionic(packet, EntityManager);
                npc.ParseNpc(packet);

                Entities.Add(npc);
            }
        }

        // Start moving
        if (Entities.LastOrDefault() is EntityBionic { Movement.Destination: not null } entity)
            entity.Movement.Start(entity.Position, entity.State.Speed);
    }

    private void ParseEventZone(Packet packet)
    {
        var eventZone = new EntityEventZone();
        eventZone.ParseEventZone(packet);
        eventZone.ParseEntity(packet);

        Entities.Add(eventZone);
    }

    private void ParsePortal(RefObjCommon refObjCommon, Packet packet)
    {
        var portal = new EntityPortal(refObjCommon);
        portal.ParseEntity(packet);
        portal.ParsePortal(packet);

        Entities.Add(portal);
    }

    private void ParseItem(RefObjItem refObjItem, Packet? packet)
    {
        var item = new EntityItem(refObjItem);
        item.ParseItem(packet);

        Entities.Add(item);
    }

    public void Clear()
    {
        Entities.Clear();
    }

    public bool TryGetEntity(uint uniqueId, out Entity? entity)
    {
        entity = Entities.FirstOrDefault(x => x.UniqueId == uniqueId);

        return entity != null;
    }

    public bool TryGetEntity<TEntityType>(uint uniqueId, out TEntityType? entity) where TEntityType : Entity
    {
        entity = Entities.FirstOrDefault(x => x.UniqueId == uniqueId) as TEntityType;

        return entity != null;
    }
}