using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.CollectionBook;

public class Theme : ReactiveObject
{
    [Reactive] public uint Index { get; internal set; }
    [Reactive] public uint StartedDateTime { get; internal set; }
    [Reactive] public uint Pages { get; internal set; }
}