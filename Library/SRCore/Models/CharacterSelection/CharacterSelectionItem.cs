using ReactiveUI;

namespace SRCore.Models.CharacterSelection
{
    public class CharacterSelectionItem : ReactiveObject
    {
        public uint RefObjId { get; internal set; }
        public byte Plus { get; internal set; }
    }
}
