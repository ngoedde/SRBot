using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using Serilog;
using SRCore.Models.CharacterSelection;
using SRGame;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class CharacterLobby(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    public delegate void CharacterListUpdatedEventHandler(CharacterLobby characterLobby);
    public event CharacterListUpdatedEventHandler? CharacterListUpdated;
    
    [Reactive] public ObservableCollection<Character> Characters { get; private set; } = new();

    private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();

    public void Join(Character character)
    {
        var packet = new Packet(AgentMsgId.CharacterListJoinReq);
        packet.WriteString(character.Name);
        
        _proxy.SendToServer(packet);
    }

    public void Request()
    {
        var packet = new Packet(AgentMsgId.CharacterListActionReq);
        packet.WriteByte(CharacterSelectionAction.List);
        
        _proxy.SendToServer(packet);
    }
    
    internal override bool TryParsePacket(Session session, Packet packet)
    {
        var action = (CharacterSelectionAction)packet.ReadByte();
        var result = (MessageResult)packet.ReadByte();

        if (result != MessageResult.Success)
            return false;

        if (action == CharacterSelectionAction.List)
        {
            Characters = [];

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
                    IsDeleting = packet.ReadBool(),
                };

                if (character.IsDeleting)
                    character.DeleteTime = packet.ReadUInt();
                
                character.GuildMemberClass = (CharacterSelectionMemberClass) packet.ReadByte();
                
                //Is guild rename required?
                if (packet.ReadBool())
                {
                    //current guild name
                    packet.ReadString();
                }
                
                character.AcademyMemberClass = (CharacterSelectionMemberClass) packet.ReadByte();
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
                
                Log.Information($"Found character {character.Name} (lv. {character.Level})");
            }
        }

        OnCharacterListUpdated(this);
        
        return true;
    }

    protected virtual void OnCharacterListUpdated(CharacterLobby characterLobby)
    {
        CharacterListUpdated?.Invoke(characterLobby);
    }
}
