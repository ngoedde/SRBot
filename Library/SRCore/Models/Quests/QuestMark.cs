using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Quests;

public class QuestMark : ReactiveObject
{
    [Reactive] public uint GameServerFrame { get; internal set; }
    [Reactive] public QuestType QuestType { get; internal set; }
    [Reactive] public QuestMarkType Type { get; internal set; }
    [Reactive] public ushort RegionId { get;internal set; }
    
    [Reactive] public Position Position { get; internal set; } = new();
    
    [Reactive] public uint NpcUniqueId { get; internal set; }
}