using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Quests;

public class QuestMark : ReactiveObject
{
    [Reactive] public uint GameServerFrame { get; internal set; }
    [Reactive] public QuestType QuestType { get; internal set; }
    [Reactive] public QuestMarkType MarkType { get; internal set; }
    [Reactive] public RegionPosition RegionPosition { get; internal set; } = new();
    [Reactive] public uint NpcUniqueId { get; internal set; }
}