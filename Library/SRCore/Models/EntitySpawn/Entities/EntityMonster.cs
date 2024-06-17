using ReactiveUI.Fody.Helpers;
using SRGame;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityMonster(RefObjChar refObjChar) : EntityNpc(refObjChar)
{
    [Reactive] public Rarity Rarity { get; set; }
    [Reactive] public byte Appearance { get; set; }

    public void ParseMonster(RefObjChar refObjChar, Packet packet)
    {
        Rarity = (Rarity)packet.ReadByte();
        
        //NPC_MOB_TIEF, NPC_MOB_HUNTER
        if (refObjChar.TypeID4 == 2 || refObjChar.TypeID4 == 3) {
            Appearance = packet.ReadByte();
        }
    }
}