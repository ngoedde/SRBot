using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using SRPack.SRAdapter.Struct;
using SRPack.SRAdapter.Utils;

namespace SRPack.SRAdapter;

internal partial class SRPack
{
    private long _lastLockedTimestampTicks;
    private bool _isLocked;
    
    public async Task<SRPackEntry> GetEntryAsync(string path)
    {
        if (_fileStream == null || !Initialized)
        {
            throw new IOException("SRPack is not initialized.");
        }
        
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

        try
        {
            await WaitForRelease();
            
            _fileStream.Seek(entry.DataPosition, SeekOrigin.Begin);
            var bytesRead = await _fileStream.ReadAsync(buffer).ConfigureAwait(false);
            if (bytesRead != buffer.Length)
            {
                throw new Exception("Unable to read file entry, file stream interruption.");
            }
            
            return buffer;
        }
        finally
        {
            Unlock();
        }
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
                CreateTime = reader.ReadInt64(),
                ModifyTime = reader.ReadInt64(),
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

        try
        {
            await WaitForRelease();   
            var folders = PathUtils.Explode(folderPath);
            foreach (var folderName in folders)
            {
                if (!currentBlock.TryGetEntry(folderName, out var entry))
                {
                    throw new IOException($"Folder {folderPath} not found.");
                }

                currentBlock = await GetOrReadBlocksAt(entry.DataPosition).ConfigureAwait(false);
            }
        }
        finally
        {
            Unlock();
        }

        return currentBlock;
    }
    
    //ORIGINAL!
    private async Task<SRPackBlock[]> GetOrReadBlocksAt(long position)
    {
        if (_index.TryGetValue(position, out var blocksFromIndex))
        {
            return blocksFromIndex;
        }
        
        var tempId = Random.Shared.Next(100);
        Debug.WriteLine($"START [{tempId}] GetOrReadBlocksAt: {position}");
        
        var blockBuffer = new byte[SRPackBlock.BlockSize];
        var block = await ReadBlockAt(position, blockBuffer).ConfigureAwait(false);
        var blockCollection = new List<SRPackBlock> { block };
        var nextBlockEntry = block.Entries[19];
    
        var sw = Stopwatch.StartNew();
        while (nextBlockEntry.NextBlock > 0)
        {
            block = await ReadBlockAt(nextBlockEntry.NextBlock, blockBuffer).ConfigureAwait(false);
            blockCollection.Add(block);
    
            nextBlockEntry = block.Entries[19];
        }
        Debug.WriteLine($"END [{tempId}]GetOrReadBlocksAt took {sw.ElapsedMilliseconds}ms for {blockCollection.Count}.");
    
        var blocks = blockCollection.ToArray();
        _index.Add(position, blocks);
    
        return blocks;
    }

    private void Lock()
    {
        _isLocked = true;
        _lastLockedTimestampTicks = DateTimeOffset.Now.Ticks;
    }

    private void Unlock()
    {
        _isLocked = false;
        _lastLockedTimestampTicks = 0;
    }
    
    private async Task WaitForRelease()
    {
        while (_isLocked && (_lastLockedTimestampTicks > 0) && DateTimeOffset.Now.Ticks - _lastLockedTimestampTicks < 5000 * 10000)
        {
            await Task.Delay(10);
        }
        
        if (_isLocked) 
            throw new IOException("SRPack: Possible deadlock detected. Reading from SRPack is locked for too long.");

        Lock();
    }
}