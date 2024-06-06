namespace SRPack.SRAdapter.Struct;

internal record SRPackBlock(SRPackEntry[] Entries, long Position)
{
    public const int BlockSize = 4096;
    public const long RootBlockPosition = 256;
}

internal static class SRPackBlockExtensions
{
    public static long GetParentBlockPosition(this IEnumerable<SRPackBlock> blocks)
    {
        var parentEntry = blocks.GetEntries().FirstOrDefault(e => e is { Name: "..", Type: SRPackEntryType.Folder });

        return parentEntry?.DataPosition ?? SRPackBlock.RootBlockPosition;
    }

    public static bool TryGetEntry(this IEnumerable<SRPackBlock> blocks, string name, out SRPackEntry entry)
    {
        try
        {
            entry = blocks.GetEntries()
                .First(entry => entry.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            return true;
        }
        catch
        {
            entry = default;
            
            return false;
        }
    }

    public static bool TryGetEntry(this IEnumerable<SRPackBlock> blocks, long dataPosition, out SRPackEntry entry)
    {
        try
        {
            entry = blocks.GetEntries()
                .First(entry => entry.DataPosition == dataPosition);

            return true;
        }
        catch
        {
            entry = default;
            
            return false;
        }
    }
    
    public static IEnumerable<SRPackEntry> GetEntries(this IEnumerable<SRPackBlock> blocks)
    {
        return blocks.SelectMany(block => block.Entries);
    }

    public static IEnumerable<SRPackEntry> GetFilesAndFolders(this IEnumerable<SRPackBlock> blocks)
    {
        return blocks.SelectMany(block => block.Entries.Where(e => e.Type != SRPackEntryType.Empty)).OrderBy(e => e.Type);
    }
    
    public static IEnumerable<SRPackEntry> GetFiles(this IEnumerable<SRPackBlock> blocks)
    {
        return blocks.SelectMany(block => block.Entries.Where(e => e.Type == SRPackEntryType.File));
    }
    
    public static IEnumerable<SRPackEntry> GetFolders(this IEnumerable<SRPackBlock> blocks)
    {
        return blocks.SelectMany(block => block.Entries.Where(e => e.Type == SRPackEntryType.Folder));
    }
    
    public static IEnumerable<SRPackEntry> GetEmpties(this IEnumerable<SRPackBlock> blocks)
    {
        return blocks.SelectMany(block => block.Entries.Where(e => e.Type == SRPackEntryType.Empty));
    }
}