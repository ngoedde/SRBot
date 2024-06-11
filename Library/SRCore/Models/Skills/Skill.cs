using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame.Client.Entity.RefObject;

namespace SRCore.Models.Skills;

public class Skill(RefSkill skill) : ReactiveObject
{
    [Reactive] public int Id { get; internal set; } = skill.Id;
    [Reactive] public bool Enabled { get; internal set; }
    [Reactive] public RefSkill RefSkill { get; internal set; } = skill;
}