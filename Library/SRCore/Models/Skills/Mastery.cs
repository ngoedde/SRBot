using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Skills;

public class Mastery : ReactiveObject
{
    [Reactive] public uint Id { get; internal set; }
    [Reactive] public byte Level { get; internal set; }
}