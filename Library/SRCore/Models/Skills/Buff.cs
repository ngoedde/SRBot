using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Skills;

public class Buff(RefSkill skill) : ReactiveObject
{
    [Reactive] public RefSkill Skill { get; internal set; } = skill;
    [Reactive] public int RefSkillId { get; internal set; } = skill.Id;
    [Reactive] public uint Duration { get; internal set; }
    [Reactive] public bool IsCreator { get; internal set; }
}