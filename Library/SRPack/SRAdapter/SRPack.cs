using SRPack.SRAdapter.Struct;
using SRPack.SRAdapter.Utils;

namespace SRPack.SRAdapter;

internal partial class SRPack
{
    public bool Initialized => _header != null && _fileStream is { CanRead: true };

    private Dictionary<long, SRPackBlock[]> _index = new();
    private Blowfish? _blowfish;
    private SRPackHeader? _header;
    private FileStream? _fileStream;

    public async Task InitializeAsync(string fileName, string password, byte[] salt)
    {
        if (Initialized)
        {
            await CloseAsync();
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new IOException($"Pack file {fileName} not found");
        }

        _index = [];
        _fileStream = File.OpenRead(fileName);
        _header = await ReadHeader();

        if (_header is { Encrypted: true })
        {
            InitializeBlowfish(password, salt);
        }

        await GetBlockAsync(string.Empty);
    }

    public async Task<KeyValuePair<string, SRPackBlock[]>> GetBlockAndPathAsync(long blockPosition)
    {
        var blocks = await GetOrReadBlocksAt(blockPosition);
        if (blocks.Length == 0)
        {
            throw new IOException($"Block at position {blockPosition} not found!");
        }

        var path = await ResolvePathFromBlock(blocks);
        return new KeyValuePair<string, SRPackBlock[]>(path, blocks);
    }

    private async Task<string> ResolvePathFromBlock(SRPackBlock[] blocks)
    {
        if (blocks.Length == 0)
        {
            throw new IOException("Blocks are empty!");
        }

        var path = string.Empty;

        //Root block
        if (blocks[0].Position == SRPackBlock.RootBlockPosition)
        {
            return string.Empty;
        }

        //Continue until "root" reached
        while (blocks[0].Position != SRPackBlock.RootBlockPosition)
        {
            var parentBlockPosition = blocks.GetParentBlockPosition();
            var parentBlocks = await GetOrReadBlocksAt(parentBlockPosition);

            if (!parentBlocks.TryGetEntry(blocks[0].Position, out var blockEntry))
            {
                throw new IOException("Block entry not found in parent block.");
            }

            path = PathUtils.Combine(blockEntry.Name, path);
            blocks = parentBlocks;
        }

        return path;
    }


    private void InitializeBlowfish(string password, byte[] salt)
    {
        var key = Blowfish.GenerateFinalBlowfishKey(password, salt);

        _blowfish = new Blowfish();
        _blowfish.Initialize(key);
    }

    public async Task CloseAsync()
    {
        if (_fileStream != null)
        {
            _fileStream.Close();
            await _fileStream.DisposeAsync().ConfigureAwait(false);
            _fileStream = null;
        }
    }
}