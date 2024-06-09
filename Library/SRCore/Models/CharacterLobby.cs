using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.CharacterSelection;
using SRGame;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models
{
    internal class CharacterLobby : GameModel
    {
        [Reactive] public ObservableCollection<Character> Characters { get; private set; } = new();


        public CharacterLobby(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        internal override bool TryParsePacket(Session session, Packet packet)
        {
            var action = (CharacterSelectionAction)packet.ReadByte();
            var result = (MessageResult)packet.ReadByte();

            if (result != MessageResult.Success)
                return false;

            if (action == CharacterSelectionAction.List)
            {
                Characters = new ObservableCollection<Character>();


                var characterCount = packet.ReadByte();
                for (var i = 0; i < characterCount; i++)
                {
                    var character = new Character
                    {
                        RefObjId = packet.ReadUInt(),
                        Name = packet.ReadString(),
                        Scale = packet.ReadByte(),
                        Level = packet.ReadByte(),
                        Experience = packet.ReadULong(),
                        Strength = packet.ReadUShort(),
                        Intelligence = packet.ReadUShort(),
                        StatPoints = packet.ReadUShort(),
                        Health = packet.ReadUInt(),
                        Mana = packet.ReadUInt(),
                        IsDeleting = packet.ReadByte(),
                        DeleteTime = packet.ReadUInt(),
                        GuildMemberClass = (CharacterSelectionMemberClass)packet.ReadByte(),
                        GuildName = packet.ReadString(),
                        AcademyMemberClass = (CharacterSelectionMemberClass)packet.ReadByte()
                    };

                    var inventoryCount = packet.ReadByte();
                    for (var j = 0; j < inventoryCount; j++)
                    {
                        character.Inventory.Add(new CharacterSelectionItem
                        {
                            RefObjId = packet.ReadUInt(),
                            Plus = packet.ReadByte()
                        });
                    }

                    var avatarInventoryCount = packet.ReadByte();
                    for (var j = 0; j < avatarInventoryCount; j++)
                    {
                        character.AvatarInventory.Add(new CharacterSelectionItem
                        {
                            RefObjId = packet.ReadUInt(),
                            Plus = packet.ReadByte()
                        });
                    }

                    Characters.Add(character);
                }

            }
        }
    }
}
