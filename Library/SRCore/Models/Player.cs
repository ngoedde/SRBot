using System.Collections.ObjectModel;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.Character;
using SRCore.Models.CollectionBook;
using SRCore.Models.EntitySpawn;
using SRCore.Models.Inventory;
using SRCore.Models.Quests;
using SRCore.Models.Skills;
using SRCore.Service;
using SRGame;
using SRGame.Client;
using SRGame.Client.Entity.RefObject;
using SRNetwork;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Player(IServiceProvider serviceProvider) : GameModel(serviceProvider)
{
    #region Properties

    [Reactive] public EntityBionic Bionic { get; internal set; }
    [Reactive] public ObservableCollection<Item> Inventory { get; internal set; } = new();
    [Reactive] public RefObjChar RefObjChar => EntityManager.GetCharacter(RefObjId)!;
    [Reactive] public ObservableCollection<Item> AvatarInventory { get; internal set; } = new();
    [Reactive] public ObservableCollection<Mastery> Masteries { get; internal set; } = new();
    [Reactive] public ObservableCollection<Skill> Skills { get; internal set; } = new();
    [Reactive] public ObservableCollection<Quest> Quests { get; internal set; } = new();
    [Reactive] public ObservableCollection<Theme> CollectionBook { get; internal set; } = new();
    [Reactive] public ObservableCollection<QuestMark> QuestMarks { get; internal set; } = new();
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
    [Reactive] public byte AutoInvestExperience { get; internal set; }
    [Reactive] public byte DailyPlayerKills { get; internal set; }
    [Reactive] public ushort TotalPlayerKills { get; internal set; }
    [Reactive] public uint PlayerKillsPenaltyPoint { get; internal set; }
    [Reactive] public byte HwanLevel { get; internal set; }
    [Reactive] public byte FrPvpMode { get; internal set; }
    [Reactive] public byte InventorySize { get; internal set; }
    [Reactive] public byte AvatarInventorySize { get; internal set; }

    [Obsolete("Use Bionic.Position instead")]
    public OrientedRegionPosition Position => Bionic.Position;

    [Obsolete("Use Bionic.State instead")]
    public State State => Bionic.State;

    [Obsolete("Use Bionic.UniqueId instead")]
    public uint UniqueId => Bionic.UniqueId;

    [Reactive] public uint[] CompletedQuests { get; internal set; } = [];
    [Reactive] public string Name { get; internal set; } = "Not in game";
    [Reactive] public Job Job { get; internal set; } = new();
    [Reactive] public PvPState PvPState { get; internal set; }
    [Reactive] public bool IsRiding { get; internal set; }
    [Reactive] public bool InCombat { get; internal set; }
    [Reactive] public uint TransportUniqueId { get; internal set; }
    [Reactive] public PvpFlagType PvpFlag { get; internal set; }
    [Reactive] public ulong GuideFlag { get; internal set; }
    [Reactive] public uint JID { get; internal set; }
    [Reactive] public bool IsGameMaster { get; internal set; }
    [Reactive] public ushort WorldId { get; internal set; }
    [Reactive] public ushort LayerId { get; internal set; }

    [Reactive] public Attributes Attributes { get; internal set; } = new();

    [Obsolete("Use property Attributes instead")]
    public Attributes Stats => Attributes;

    internal Packet CharacterDataPacket { get; set; } = new(AgentMsgId.CharacterData);
    private EntityManager EntityManager => serviceProvider.GetRequiredService<EntityManager>();
    private Proxy Proxy => serviceProvider.GetRequiredService<Proxy>();

    #endregion

    internal override void ParsePacket(Session session, Packet packet)
    {
        Inventory.Clear();
        AvatarInventory.Clear();
        Masteries.Clear();
        Skills.Clear();
        Quests.Clear();
        CollectionBook.Clear();
        QuestMarks.Clear();

        // Inventory = new ObservableCollection<Item>();
        // AvatarInventory = new ObservableCollection<Item>();
        // Masteries = new ObservableCollection<Mastery>();
        // Skills = new ObservableCollection<Skill>();
        // Quests = new ObservableCollection<Quest>();
        // CollectionBook = new ObservableCollection<Theme>();
        // QuestMarks = new ObservableCollection<QuestMark>();

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
        AutoInvestExperience = packet.ReadByte();
        DailyPlayerKills = packet.ReadByte();
        TotalPlayerKills = packet.ReadUShort();
        PlayerKillsPenaltyPoint = packet.ReadUInt();
        HwanLevel = packet.ReadByte();
        FrPvpMode = packet.ReadByte();

        //Inventory
        InventorySize = packet.ReadByte();
        var inventoryCount = packet.ReadByte();
        var items = Enumerable.Range(0, inventoryCount)
            .Select(_ => ItemFactory.ParseFromPacket(packet, EntityManager))
            .ToList();
        Inventory.AddRange(items);

        //Avatar Inventory
        AvatarInventorySize = packet.ReadByte();
        var avatarInventoryCount = packet.ReadByte();
        for (var i = 0; i < avatarInventoryCount; i++)
            AvatarInventory.Add(ItemFactory.ParseFromPacket(packet, EntityManager));

        packet.ReadByte(); //Unknown

        //Masteries
        var nextMastery = packet.ReadByte();
        while (nextMastery == 1)
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
        while (nextSkill == 1)
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
            Quests.Add(QuestFactory.ParseFromPacket(packet));

        var questMarkCount = packet.ReadByte();
        for (var i = 0; i < questMarkCount; i++)
        {
            QuestMarks.Add(new QuestMark
            {
                GameServerFrame = packet.ReadUInt(),
                QuestType = (QuestType)packet.ReadByte(),
                MarkType = (QuestMarkType)packet.ReadByte(),
                RegionPosition = RegionPosition.FromPacket(packet),
                NpcUniqueId = packet.ReadUInt()
            });
        }

        CollectionBook = Character.CollectionBook.FromPacket(packet);

        Bionic = new EntityBionic(RefObjChar);
        Bionic.ParseEntity(packet);
        Bionic.ParseBionic(packet, EntityManager);

        Name = packet.ReadString();
        Job = Job.FromPacket(packet);
        PvPState = (PvPState)packet.ReadByte();
        IsRiding = packet.ReadBool();
        InCombat = packet.ReadBool();

        if (IsRiding)
            TransportUniqueId = packet.ReadUInt();

        PvpFlag = (PvpFlagType)packet.ReadByte();
        GuideFlag = packet.ReadULong();
        JID = packet.ReadUInt();
        IsGameMaster = packet.ReadBool();

        //Skip hotkeys & auto potions part:
        packet.ReadByte(); //Activation Flag
        var hotkeyCount = packet.ReadByte();
        for (var i = 0; i < hotkeyCount; i++)
        {
            packet.ReadByte(); //Sequence
            packet.ReadByte(); //ContentType
            packet.ReadUInt(); //Data
        }

        packet.ReadUShort(); //HPConfig
        packet.ReadUShort(); //MPConfig
        packet.ReadUShort(); //UniversalConfig
        packet.ReadByte(); //Delay

        var blockedWhisperCount = packet.ReadByte();
        for (var i = 0; i < blockedWhisperCount; i++)
            packet.ReadString(); //Blocked name

        WorldId = packet.ReadUShort();
        LayerId = packet.ReadUShort();
    }
}