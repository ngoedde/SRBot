using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.CollectionBook;
using SRCore.Models.Inventory;
using SRCore.Models.Quests;
using SRCore.Models.Skills;
using SRGame.Client;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Character(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    [Reactive] public uint RefObjId { get; internal set; }
    [Reactive] public byte Scale { get; internal set; }
    [Reactive] public byte Level { get; internal set; }
    [Reactive] public byte MaxLevel { get; internal set; }
    [Reactive] public ulong Experience { get; internal set; }
    [Reactive] public uint SkillExperience { get; internal set; }
    [Reactive] public ulong Gold { get; internal set; }
    
    [Reactive] public uint SkillPoints { get; internal set; }
    [Reactive] public ushort StatPoints { get; internal set; }
    [Reactive] public byte Hwan { get; internal set; }
    [Reactive] public uint GatheredExperience { get; internal set; }
    [Reactive] public uint Health { get; internal set; }
    [Reactive] public uint Mana { get; internal set; }
    [Reactive] public byte AutoInverstExperience { get; internal set; }
    [Reactive] public byte DailyPlayerKills { get; internal set; }
    [Reactive] public ushort TotalPlayerKills { get; internal set; }
    [Reactive] public uint PlayerKillsPenaltyPoint { get; internal set; }
    [Reactive] public byte HwanLevel { get; internal set; }
    [Reactive] public byte PvpMode { get; internal set; }
    [Reactive] public byte InventorySize { get; internal set; }
    [Reactive] public byte AvatarInventorySize { get; internal set; }

    [Reactive] public Movement Movement { get; internal set; } = new();

    [Reactive] public EntityPosition Position { get; internal set; } = new();
    
    [Reactive] public ObservableCollection<Item> Inventory { get; internal set; } = new();
    [Reactive] public ObservableCollection<Item> AvatarInventory { get; internal set; } = new();
    [Reactive] public ObservableCollection<Mastery> Masteries { get; internal set; } = new();
    
    [Reactive] public ObservableCollection<Skill> Skills { get; internal set; } = new();
    [Reactive] public ObservableCollection<Quest> Quests { get; internal set; } = new();
    
    [Reactive] public ObservableCollection<Theme> CollectionBook { get; internal set; } = new();
    [Reactive] public uint[] CompletedQuests { get; internal set; } = [];
    
    [Reactive] public uint UniqueId { get; internal set; }
    
    internal Packet CharacterDataPacket { get; set; } = new(AgentMsgId.CharacterData);
    
    private EntityManager EntityManager => serviceProvider.GetRequiredService<EntityManager>();
    
    internal override void ParsePacket(Session session, Packet packet)
    {
        Inventory = new ObservableCollection<Item>();
        AvatarInventory = new ObservableCollection<Item>();
        Masteries = new ObservableCollection<Mastery>();
        Skills = new ObservableCollection<Skill>();
        Quests = new ObservableCollection<Quest>();
        CollectionBook = new ObservableCollection<Theme>();
        
        
        //Server time for some reason. Don't need this.
        packet.ReadUInt();
        
        RefObjId = packet.ReadUInt();
        Scale = packet.ReadByte();
        Level = packet.ReadByte();
        MaxLevel = packet.ReadByte();
        Experience = packet.ReadULong();
        SkillExperience = packet.ReadUInt();
        Gold = packet.ReadULong();
        SkillPoints = packet.ReadUInt();
        StatPoints = packet.ReadUShort();
        Hwan = packet.ReadByte();
        GatheredExperience = packet.ReadUInt();
        Health = packet.ReadUInt();
        Mana = packet.ReadUInt();
        AutoInverstExperience = packet.ReadByte();
        DailyPlayerKills = packet.ReadByte();
        TotalPlayerKills = packet.ReadUShort();
        PlayerKillsPenaltyPoint = packet.ReadUInt();
        HwanLevel = packet.ReadByte();
        PvpMode = packet.ReadByte();
        
        //Inventory
        InventorySize = packet.ReadByte();
        var inventoryCount = packet.ReadByte();
        for (var i = 0; i < inventoryCount; i++)
        {
            Inventory.Add(ItemFactory.ParseFromPacket(packet, EntityManager));
        }
        
        //Avatar Inventory
        AvatarInventorySize = packet.ReadByte();
        var avatarInventoryCount = packet.ReadByte();
        for (var i = 0; i < avatarInventoryCount; i++)
        {
            AvatarInventory.Add(ItemFactory.ParseFromPacket(packet, EntityManager));
        }

        packet.ReadByte(); //Unknown

        //Masteries
        var nextMastery = packet.ReadByte();
        while(nextMastery == 1)
        {
            var mastery = new Mastery
            {
                Id = packet.ReadUInt(),
                Level = packet.ReadByte()
            };
            Masteries.Add(mastery);
            
            nextMastery = packet.ReadByte();
        }

        packet.ReadByte(); //Unknown
        
        //Skills
        var nextSkill = packet.ReadByte();
        while(nextSkill == 1)
        {
            var skillId = packet.ReadInt();
            var refSkill = EntityManager.GetSkill(skillId);
            if (refSkill == null)
                throw new Exception($"Skill with id {skillId}not found");

            var skill = new Skill(refSkill)
            {
                Enabled = packet.ReadBool()
            };
            
            Skills.Add(skill);
            
            nextSkill = packet.ReadByte();
        }

        var completedQuestCount = packet.ReadUShort();
        CompletedQuests = packet.ReadUIntArray(completedQuestCount);
        
        var activeQuestCount = packet.ReadByte();
        for (var i = 0; i < activeQuestCount; i++)
        {
            var quest = QuestFactory.ParseFromPacket(packet);
            Quests.Add(quest);
        }
        
        var collectionBookCount = packet.ReadUInt();
        for (var i = 0; i < collectionBookCount; i++)
        {
            var theme = new Theme
            {
                Index = packet.ReadUInt(),
                StartedDateTime = packet.ReadUInt(),
                Pages = packet.ReadUInt(),
            };
            CollectionBook.Add(theme);
        }

        Position = EntityPosition.FromPacket(packet);
        Movement = Movement.FromPacket(packet);
        
    }
}








