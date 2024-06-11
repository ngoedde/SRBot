using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Quests;

public class Quest : ReactiveObject
{
    [Reactive] public uint RefQuestId { get; internal set; }
    [Reactive] public byte AchievementCount { get; internal set; }
    [Reactive] public bool RequiresAutoShareParty { get; internal set; }
    [Reactive] public QuestType Type { get; internal set; }
    [Reactive] public uint RemainingTime { get; internal set; }
    [Reactive] public QuestStatus Status { get; set; } = QuestStatus.Achieving;
    [Reactive] public ObservableCollection<QuestObjective> Objectives { get; internal set; } = new();
    [Reactive] public uint[] NpcIds { get; internal set; } = [];
    [Reactive] public ObservableCollection<QuestMark> Marks { get; internal set; } = new();
}