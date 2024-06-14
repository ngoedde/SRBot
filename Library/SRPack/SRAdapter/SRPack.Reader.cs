using SRPack.SRAdapter.Struct;
using SRPack.SRAdapter.Utils;

namespace SRPack.SRAdapter;

internal partial class SRPack
{
    public async Task<SRPackEntry> GetEntryAsync(string path)
    {
        path = PathUtils.Normalize(path);

        var fileName = PathUtils.GetFileName(path);
        var folderName = PathUtils.GetParent(path);

        var blocks = await GetBlockAsync(folderName).ConfigureAwait(false);
        if (!blocks.TryGetEntry(fileName, out var entry))
        {
            throw new IOException($"File {path} not found.");
        }

        return entry;
    }

    public async Task<byte[]> ReadFileEntryAsync(SRPackEntry entry)
    {
        if (_fileStream == null || !Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        var buffer = new byte[entry.Size];

        _fileStream.Seek(entry.DataPosition, SeekOrigin.Begin);
        var bytesRead = await _fileStream.ReadAsync(buffer).ConfigureAwait(false);
        if (bytesRead != buffer.Length)
        {
            throw new Exception("Unable to read file entry, file stream interruption.");
        }

        return buffer;
    }

    private async Task<SRPackHeader> ReadHeader()
    {
        if (_fileStream == null)
        {
            throw new InvalidOperationException("File stream is not initialized");
        }

        var headerBuffer = new byte[SRPackHeader.HeaderSize];
        var actualHeaderLengthRead = await _fileStream.ReadAsync(headerBuffer).ConfigureAwait(false);

        if (actualHeaderLengthRead != SRPackHeader.HeaderSize)
        {
            throw new InvalidDataException("Invalid header size or file stream interruption, unable to read header.");
        }

        using var reader = new BinaryReader(new MemoryStream(headerBuffer));
        return new SRPackHeader
        {
            Signature = reader.ReadString(30),
            Version = reader.ReadBytes(4),
            Encrypted = Convert.ToBoolean(reader.ReadByte()),
            EncryptionChecksum = reader.ReadBytes(16),
            Payload = reader.ReadBytes(205)
        };
    }

    private async Task<SRPackBlock> ReadBlockAt(long position, byte[] buffer)
    {
        if (_fileStream == null || !Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }

        _fileStream.Seek(position, SeekOrigin.Begin);

        var bytesRead = await _fileStream.ReadAsync(buffer.AsMemory()).ConfigureAwait(false);

        if (bytesRead != SRPackBlock.BlockSize)
        {
            throw new IOException("Invalid block size or file stream interruption, unable to read block.");
        }

        if (_blowfish != null)
        {
            var decoded = new byte[buffer.Length];

            _blowfish.Decode(buffer.AsMemory(0, bytesRead), decoded);

            buffer = decoded;
        }

        using var reader = new BinaryReader(new MemoryStream(buffer));

        var entries = new SRPackEntry[20];
        for (var i = 0; i < 20; i++)
        {
            entries[i] = new SRPackEntry
            {
                Type = (SRPackEntryType)reader.ReadByte(),
                Name = reader.ReadString(89),
                CreateTime = DateTime.FromFileTimeUtc(reader.ReadInt64()),
                ModifyTime = DateTime.FromFileTimeUtc(reader.ReadInt64()),
                DataPosition = reader.ReadInt64(),
                Size = reader.ReadUInt32(),
                NextBlock = reader.ReadInt64(),
                Payload = reader.ReadBytes(2) //Padding to reach 128 bytes length
            };
        }

        return new SRPackBlock(entries, position);
    }

    public async Task<SRPackBlock[]> GetBlockAsync(string folderPath)
    {
        //Resolve root if not done yet.
        if (folderPath == string.Empty && !_index.TryGetValue(SRPackBlock.RootBlockPosition, out _))
        {
            var rootBlocks = await GetOrReadBlocksAt(SRPackBlock.RootBlockPosition).ConfigureAwait(false);

            return rootBlocks;
        }

        folderPath = PathUtils.Normalize(folderPath);
        var currentBlock = _index[SRPackBlock.RootBlockPosition];

        if (folderPath == string.Empty)
        {
            return currentBlock;
        }

        var folders = PathUtils.Explode(folderPath);
        foreach (var folderName in folders)
        {
            if (!currentBlock.TryGetEntry(folderName, out var entry))
            {
                throw new IOException($"Folder {folderPath} not found.");
            }

            currentBlock = await GetOrReadBlocksAt(entry.DataPosition).ConfigureAwait(false);
        }

        return currentBlock;
    }

    private async Task<SRPackBlock[]> GetOrReadBlocksAt(long position)
    {
        if (_index.TryGetValue(position, out var blocksFromIndex))
        {
            return blocksFromIndex;
        }

        var blockBuffer = new byte[SRPackBlock.BlockSize];
        var block = await ReadBlockAt(position, blockBuffer).ConfigureAwait(false);
        var blockCollection = new List<SRPackBlock> { block };
        var nextBlockEntry = block.Entries[19];

        while (nextBlockEntry.NextBlock > 0)
        {
            block = await ReadBlockAt(nextBlockEntry.NextBlock, blockBuffer).ConfigureAwait(false);
            blockCollection.Add(block);

            nextBlockEntry = block.Entries[19];
        }

        var blocks = blockCollection.ToArray();
        _index.Add(position, blocks);

        return blocks;
    }
}