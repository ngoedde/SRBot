using SRPack.SRAdapter.Struct;
using SRPack.SRAdapter.Utils;

namespace SRPack;

public class SRFileAdapter(string fileName, string password, byte[] salt) : IAsyncFileAdapter
{
    #region Properties

    public bool IsInitialized => _pack.Initialized;

    public string FileName { get; } = fileName;

    #endregion

    private readonly SRAdapter.SRPack _pack = new();

    public SRFileAdapter(string fileName, string password = "169841") : this(fileName, password,
        [0x03, 0xF8, 0xE4, 0x44, 0x88, 0x99, 0x3F, 0x64, 0xFE, 0x35])
    {
    }

    public async Task InitializeAsync()
    {
        await _pack.InitializeAsync(FileName, password, salt);
    }

    public async Task CloseAsync()
    {
        await _pack.CloseAsync();
    }

    public async Task<IEnumerable<string>> GetFilesAsync(string folderPath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var blocks = await _pack.GetBlockAsync(folderPath);
        return blocks.GetFiles().Select(f => f.Name);
    }

    public async Task<IEnumerable<string>> GetFoldersAsync(string folderPath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var blocks = await _pack.GetBlockAsync(folderPath);
        return blocks.GetFolders().Select(f => f.Name);
    }

    public async Task<IEnumerable<string>> GetFilesAndFoldersAsync(string folderPath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var blocks = await _pack.GetBlockAsync(folderPath);
        return blocks.GetFilesAndFolders().Select(f => f.Name);
    }

    public async Task<IEnumerable<FileSystemEntryInfo>> GetEntriesAsync(long blockPosition)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var path = await _pack.GetBlockAndPathAsync(blockPosition);
        return path.Value.GetFilesAndFolders().Select(f => new FileSystemEntryInfo(PathUtils.Combine(path.Key, f.Name))
        {
            DataSize = f.Size,
            DataPosition = f.DataPosition,
            CreateTime = f.CreateTime,
            ModifyTime = f.ModifyTime,
            Type = f.Type == SRPackEntryType.File ? FileSystemEntryType.File : FileSystemEntryType.Folder
        });
    }

    public async Task<MemoryStream> ReadAllAsync(string filePath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var entry = await _pack.GetEntryAsync(filePath);
        if (entry is not { Type: SRPackEntryType.File })
        {
            throw new IOException($"File {filePath} not found.");
        }

        var buffer = await _pack.ReadFileEntryAsync(entry);

        return new MemoryStream(buffer, false);
    }

    public async Task<string> ReadAllTextAsync(string filePath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var stream = await ReadAllAsync(filePath);

        return await new StreamReader(stream).ReadToEndAsync();
    }

    public async Task<byte[]> ReadAllBytesAsync(string filePath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var stream = await ReadAllAsync(filePath);

        return stream.GetBuffer();
    }

    public async Task<bool> ExistsAsync(string filePath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        try
        {
            await _pack.GetEntryAsync(filePath);
        }
        catch (IOException)
        {
            return false;
        }

        return true;
    }

    public async Task<FileSystemEntryInfo> GetEntryAsync(string path)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var entry = await _pack.GetEntryAsync(path);

        return new FileSystemEntryInfo(path)
        {
            DataSize = entry.Size,
            DataPosition = entry.DataPosition,
            CreateTime = entry.CreateTime,
            ModifyTime = entry.ModifyTime,
            Type = entry.Type == SRPackEntryType.File ? FileSystemEntryType.File : FileSystemEntryType.Folder
        };
    }

    public async Task<IEnumerable<FileSystemEntryInfo>> GetEntriesAsync(string folderPath)
    {
        if (!_pack.Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var blocks = await _pack.GetBlockAsync(folderPath);
        return blocks.GetFilesAndFolders().Select(f => new FileSystemEntryInfo(PathUtils.Combine(folderPath, f.Name))
        {
            DataSize = f.Size,
            DataPosition = f.DataPosition,
            CreateTime = f.CreateTime,
            ModifyTime = f.ModifyTime,
            Type = f.Type == SRPackEntryType.File ? FileSystemEntryType.File : FileSystemEntryType.Folder
        });
    }
}