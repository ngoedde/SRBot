using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using Serilog;
using SRCore.Models.CharacterSelection;
using SRCore.Models.Inventory;
using SRGame;
using SRGame.Client;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class CharacterLobby(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    public delegate void CharacterListUpdatedEventHandler(CharacterLobby characterLobby);

    public event CharacterListUpdatedEventHandler? CharacterListUpdated;

    [Reactive] public ObservableCollection<CharacterSelection.Character> Characters { get; private set; } = new();

    private readonly Proxy _proxy = serviceProvider.GetRequiredService<Proxy>();
    private readonly EntityManager _entityManager = serviceProvider.GetRequiredService<EntityManager>();

    public void Join(CharacterSelection.Character character)
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

    internal override void ParsePacket(Session session, Packet packet)
    {
        var action = (CharacterSelectionAction)packet.ReadByte();
        var result = (MessageResult)packet.ReadByte();

        if (result != MessageResult.Success)
            return;

        if (action == CharacterSelectionAction.List)
        {
            Characters = [];

            var characterCount = packet.ReadByte();
            for (var i = 0; i < characterCount; i++)
            {
                var character = new CharacterSelection.Character
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

                character.GuildMemberClass = (CharacterSelectionMemberClass)packet.ReadByte();

                //Is guild rename required?
                if (packet.ReadBool())
                {
                    //current guild name
                    packet.ReadString();
                }

                character.AcademyMemberClass = (CharacterSelectionMemberClass)packet.ReadByte();
                var inventoryCount = packet.ReadByte();
                for (var j = 0; j < inventoryCount; j++)
                {
                    var refObjItemId = packet.ReadInt();
                    var refObjItem = _entityManager.GetItem(refObjItemId);
                    if (refObjItem == null)
                    {
                        throw new Exception($"Item {refObjItemId} not found in entity manager.");
                    }
                    
                    character.Inventory.Add(new InventoryItemBasic(refObjItem)
                    {
                        OptLevel = packet.ReadByte()
                    });
                }

                var avatarInventoryCount = packet.ReadByte();
                for (var j = 0; j < avatarInventoryCount; j++)
                {
                    var refObjItemId = packet.ReadInt();
                    var refObjItem = _entityManager.GetItem(refObjItemId);
                    if (refObjItem == null)
                    {
                        throw new Exception($"Item {refObjItemId} not found in entity manager.");
                    }
                    
                    character.AvatarInventory.Add(new InventoryItemBasic(refObjItem)
                    {
                        OptLevel = packet.ReadByte()
                    });
                }

                Characters.Add(character);

                Log.Information($"Found character {character.Name} (lv. {character.Level})");
            }
        }

        OnCharacterListUpdated(this);
    }

    protected virtual void OnCharacterListUpdated(CharacterLobby characterLobby)
    {
        CharacterListUpdated?.Invoke(characterLobby);
    }
}