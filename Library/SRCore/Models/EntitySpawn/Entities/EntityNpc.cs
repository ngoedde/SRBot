using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityNpc(RefObjChar refObjChar) : EntityBionic(refObjChar)
{
    [Reactive] public EntityDialog Dialog { get; set; } = new EntityDialog();
    
    public void ParseNpc(Packet packet)
    {
        Dialog = new EntityDialog
        {
            TalkFlag = packet.ReadByte()
        };

        if (Dialog.TalkFlag == 2)
        {
            var talkOptionsCount = packet.ReadByte();
            packet.ReadByteArray(talkOptionsCount);
        }
    }
}