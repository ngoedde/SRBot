using SRPack.SRAdapter.Utils;

namespace SRPack;

public class FileSystemEntryInfo(string path)
{
    public string Name { get; } = PathUtils.GetFileName(path);
    public string Path { get; } = path;
    public uint DataSize { get; init; } = 0;
    public long DataPosition { get; init; } = 0;
    public long CreateTime { get; init; } = 0;
    public long ModifyTime { get; init; } = 0;
    public FileSystemEntryType Type { get; init; } = FileSystemEntryType.Unsupported;
}