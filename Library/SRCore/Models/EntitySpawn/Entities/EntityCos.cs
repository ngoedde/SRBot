using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityCos(RefObjChar refObjChar) : EntityBionic(refObjChar)
{
    [Reactive] public override string Name { get; internal set; } = string.Empty;
    [Reactive] public string? OwnerName { get; internal set; }
    [Reactive] public uint OwnerUniqueId { get; internal set; }
    [Reactive] public byte OwnerJobType { get; internal set; }
    [Reactive] public byte OwnerPvpState { get; internal set; }
    [Reactive] public string? GuildName { get; internal set; }

    public void ParseCos(Packet packet)
    {
        if (refObjChar.TypeID4 == 2 || //NPC_COS_TRANSPORT
            refObjChar.TypeID4 == 3 || //NPC_COS_P_GROWTH
            refObjChar.TypeID4 == 4 || //NPC_COS_P_ABILITY
            refObjChar.TypeID4 == 5 || //NPC_COS_GUILD
            refObjChar.TypeID4 == 6 || //NPC_COS_CAPTURED
            refObjChar.TypeID4 == 7 || //NPC_COS_QUEST
            refObjChar.TypeID4 == 8) //NPC_COS_QUEST
        {
            if (refObjChar.TypeID4 == 3 || //NPC_COS_P_GROWTH
                refObjChar.TypeID4 == 4) //NPC_COS_P_ABILITY
            {
                Name = packet.ReadString();
            }
            else if (refObjChar.TypeID4 == 5) //NPC_COS_GUILD
            {
                GuildName = packet.ReadString();
            }

            if (refObjChar.TypeID4 == 2 || //NPC_COS_TRANSPORT
                refObjChar.TypeID4 == 3 || //NPC_COS_P_GROWTH
                refObjChar.TypeID4 == 4 || //NPC_COS_P_ABILITY
                refObjChar.TypeID4 == 5 || //NPC_COS_GUILD
                refObjChar.TypeID4 == 6) //NPC_COS_CAPTURED
            {
                OwnerName = packet.ReadString();

                if (refObjChar.TypeID4 == 2 || //NPC_COS_TRANSPORT
                    refObjChar.TypeID4 == 3 || //NPC_COS_P_GROWTH
                    refObjChar.TypeID4 == 4 || //NPC_COS_P_ABILITY
                    refObjChar.TypeID4 == 5) //NPC_COS_GUILD
                {
                    OwnerJobType = packet.ReadByte();
                    if (refObjChar.TypeID4 == 2 || //NPC_COS_TRANSPORT
                        refObjChar.TypeID4 == 3 || //NPC_COS_P_GROWTH
                        refObjChar.TypeID4 == 5) //NPC_COS_GUILD
                    {
                        OwnerPvpState = packet.ReadByte();
                        if (refObjChar.TypeID4 == 5)
                        {
                            packet.ReadUInt();//COS.Owner.RefObjId
                        }
                    }
                }
            }

            OwnerUniqueId = packet.ReadUInt();
        }
    }
}