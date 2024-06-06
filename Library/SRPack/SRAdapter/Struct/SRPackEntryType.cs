namespace SRPack.SRAdapter.Struct;

[Flags]
internal enum SRPackEntryType : byte
{
    Empty = 0x00,
    Folder = 0x01,
    File = 0x02
}