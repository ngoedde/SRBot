using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SRCore.Models;

public class EntityDialog : ReactiveObject
{
   [Reactive] public byte TalkFlag { get; set; }
   [Reactive] public byte[] TalkOptions { get; set; } = [];
}