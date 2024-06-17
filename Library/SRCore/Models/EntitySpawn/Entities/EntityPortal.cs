using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.EntitySpawn.Entities;

public class EntityPortal(RefObjCommon refObjCommon) : Entity(refObjCommon)
{
   [Reactive] public string OwnerName { get; internal set; }
   [Reactive] public uint OwnerUniqueId { get; internal set; }

   public void ParsePortal(Packet packet)
   {
       packet.ReadByte(); //Teleporter.unkByte0
       var unkByte1 = packet.ReadByte(); //Teleporter.unkByte1
       packet.ReadByte(); //Teleporter.unkByte2
       var unkByte3 =packet.ReadByte(); //Teleporter.unkByte3
       if (unkByte3 == 1)
       {
           packet.ReadInt();
           packet.ReadInt();
       }
       else if (unkByte3 == 6)
       {
           OwnerName = packet.ReadString();
           OwnerUniqueId = packet.ReadUInt();
       }
       
       if(unkByte1 == 1)
       {
           packet.ReadInt();
           packet.ReadByte();
       }
       
   }
}