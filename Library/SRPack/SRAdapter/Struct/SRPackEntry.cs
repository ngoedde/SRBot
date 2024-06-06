namespace SRPack.SRAdapter.Struct;

internal record SRPackEntry
{
    public const int EntrySize = 128;
    public SRPackEntryType Type = SRPackEntryType.Empty;
    public string Name = string.Empty;
    public DateTime CreateTime = DateTime.MinValue;
    public DateTime ModifyTime = DateTime.MinValue;
    public long DataPosition = 0;
    public uint Size = 0;
    public long NextBlock = 0;
    public byte[] Payload = new byte[2];
}