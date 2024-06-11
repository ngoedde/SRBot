using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRGame;

namespace SRCore.Models.CharacterSelection
{
    public class Character : ReactiveObject
    {
        [Reactive] public uint RefObjId { get; internal set; }
        [Reactive] public string Name { get; internal set; }
        [Reactive] public byte Scale { get; internal set; }
        [Reactive] public byte Level { get; internal set; }
        [Reactive] public ulong Experience { get; internal set; }
        [Reactive] public ushort Strength { get; internal set; }
        [Reactive] public ushort Intelligence { get; internal set; }
        [Reactive] public ushort StatPoints { get; internal set; }
        [Reactive] public uint Health { get; internal set; }
        [Reactive] public uint Mana { get; internal set; }
        [Reactive] public bool IsDeleting { get; internal set; }
        [Reactive] public uint DeleteTime { get; internal set; } //in Minutes
        [Reactive] public CharacterSelectionMemberClass GuildMemberClass { get; internal set; }
        [Reactive] public CharacterSelectionMemberClass AcademyMemberClass { get; internal set; }
        [Reactive] public ObservableCollection<CharacterSelectionItem> Inventory { get; internal set; } = new();
        [Reactive] public ObservableCollection<CharacterSelectionItem> AvatarInventory { get; internal set; } = new();
    }
}
