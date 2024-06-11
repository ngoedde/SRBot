using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Quests;

public class QuestObjective : ReactiveObject
{
    [Reactive] public byte Id { get; internal set; }
    [Reactive] public QuestObjectiveStatus Status { get; internal set; }
    [Reactive] public string Name { get; internal set; } = string.Empty;
    [Reactive] public uint[] TaskValues { get; internal set; } = [];
}