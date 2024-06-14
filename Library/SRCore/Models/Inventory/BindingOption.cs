using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models.Inventory;

public class BindingOption : ReactiveObject
{
    [Reactive] public byte Slot { get; internal set; }
    [Reactive] public uint Id { get; internal set; }
    [Reactive] public uint Value { get; internal set; }
}